using UnityEngine;
using Unity.InferenceEngine;

public class RunInference : MonoBehaviour
{
    [Header("Model")]
    [SerializeField] private ModelAsset modelAsset;
    [SerializeField] private TextAsset imageNetTags;

    [Header("Input")]
    [SerializeField] private Texture2D inputImage;

    [Header("Output")]
    [SerializeField] private TMPro.TextMeshProUGUI textUI;

    private Worker worker;
    private string[] tags;

    // Normalización estándar de ImageNet
    private static readonly float[] mean = { 0.485f, 0.456f, 0.406f };
    private static readonly float[] std = { 0.229f, 0.224f, 0.225f };

    void Start()
    {
        tags = imageNetTags.text.Split('\n');
        var runtimeModel = ModelLoader.Load(modelAsset);
        worker = new Worker(runtimeModel, BackendType.GPUCompute);

        var inputTensor = TexturaATensor(inputImage);
        worker.Schedule(inputTensor);

        var output = worker.PeekOutput() as Tensor<float>;
        var logits = output.DownloadToArray(); // 1000 floats

        int claseTop = ArgMax(logits);
        Debug.Log($"Clase predicha: {claseTop} (logit: {logits[claseTop]})");
        ReaccionarAlResultado(claseTop);

        inputTensor.Dispose();
        output.Dispose();
    }

    Tensor<float> TexturaATensor(Texture2D tex)
    {
        TensorShape shape = new TensorShape(1, 3, 224, 224);

        TextureTransform transform = new TextureTransform()
            .SetTensorLayout(TensorLayout.NCHW)
            .SetCoordOrigin(CoordOrigin.TopLeft); // corrige el flip vertical de GetPixels

        Tensor<float> tensor = new Tensor<float>(shape);
        TextureConverter.ToTensor(tex, tensor, transform);
        return tensor;
    }

    int ArgMax(float[] arr)
    {
        int idx = 0;
        for (int i = 1; i < arr.Length; i++)
            if (arr[i] > arr[idx]) idx = i;
        return idx;
    }

    void ReaccionarAlResultado(int clase)
    {
        string nombreClase = tags[clase].Trim();
        textUI.text = $"Detecté: {nombreClase}";
    }

    void OnDestroy() => worker?.Dispose();
}

