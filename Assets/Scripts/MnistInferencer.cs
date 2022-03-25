using System;
using System.Linq;
using Unity.Barracuda;
using UnityEngine;

public class MnistInferencer : IDisposable
{
    private readonly IWorker _worker;
    
    public MnistInferencer(NNModel model)
    {
        // 利用する際はrun-time model(Model型)に変換する
        var runtimeModel = ModelLoader.Load(model);
        // Workerを作成する
        _worker = WorkerFactory.CreateWorker(runtimeModel);
    }

    /// <summary>
    /// 推論をする
    /// </summary>
    /// <returns>一番確率の大きい数字</returns>
    public Result Execute(float[,] input)
    {
        // Mnistは28x28のグレースケールな画像、画像を1x28x28x1のTensorに変換する
        using var inputTensor = new Tensor(n: 1, h: 28, w: 28, c: 1);

        for (var y = 0; y < 28; y++)
        {
            for (var x = 0; x < 28; x++)
            {
                inputTensor[0, 27 - y, x, 0] = input[x, y];
            }
        }
        
        // 推論を実行する
        _worker.Execute(inputTensor);

        // 出力を取得する
        var output = _worker.PeekOutput();
        
        // 0 ~ 1の確率に
        var scores = Enumerable.Range(0, 10)
            .Select(i => output[0, 0, 0, i])
            .SoftMax()
            .ToArray();

        // 確率が一番大きい数字を探す
        var max = 0f;
        var maxIndex = 0;
        for (var i = 0; i < scores.Length; i++)
        {
            if (scores[i] > max)
            {
                max = scores[i];
                maxIndex = i;
            }
        }
        
        return new Result(maxIndex, max);
    }

    public void Dispose()
    {
        _worker?.Dispose();
    }
    
    /// <summary>
    /// 推論結果
    /// </summary>
    public readonly struct Result
    {
        public int Number { get; }
        public float Score { get; }

        public Result(int number, float score)
        {
            Number = number;
            Score = score;
        }
    }
}
