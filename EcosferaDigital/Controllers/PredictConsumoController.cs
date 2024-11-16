using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Collections.Generic;

namespace EcosferaDigital.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EnergiaPredictController : ControllerBase
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;

        public EnergiaPredictController()
        {
            _mlContext = new MLContext();
            TrainModel(); // Treina o modelo ao iniciar o controlador
        }

        private void TrainModel()
        {
            // Dados de exemplo para treinamento
            var data = new List<EnergiaData>
            {
                new EnergiaData { ConsumoKWH = 100, GeracaoKWH = 150, ProximoConsumoKWH = 120 },
                new EnergiaData { ConsumoKWH = 200, GeracaoKWH = 250, ProximoConsumoKWH = 220 },
                new EnergiaData { ConsumoKWH = 300, GeracaoKWH = 350, ProximoConsumoKWH = 320 },
                new EnergiaData { ConsumoKWH = 400, GeracaoKWH = 450, ProximoConsumoKWH = 420 },
                new EnergiaData { ConsumoKWH = 500, GeracaoKWH = 550, ProximoConsumoKWH = 520 },
                new EnergiaData { ConsumoKWH = 600, GeracaoKWH = 650, ProximoConsumoKWH = 620 },
                new EnergiaData { ConsumoKWH = 700, GeracaoKWH = 750, ProximoConsumoKWH = 720 },
                new EnergiaData { ConsumoKWH = 800, GeracaoKWH = 850, ProximoConsumoKWH = 820 },
                new EnergiaData { ConsumoKWH = 900, GeracaoKWH = 950, ProximoConsumoKWH = 920 },
                new EnergiaData { ConsumoKWH = 1000, GeracaoKWH = 1050, ProximoConsumoKWH = 1020 }
            };

            var trainingData = _mlContext.Data.LoadFromEnumerable(data);

            var pipeline = _mlContext.Transforms.Concatenate("Features", nameof(EnergiaData.ConsumoKWH), nameof(EnergiaData.GeracaoKWH))
                .Append(_mlContext.Regression.Trainers.Sdca(labelColumnName: nameof(EnergiaData.ProximoConsumoKWH), maximumNumberOfIterations: 100));

            _model = pipeline.Fit(trainingData);
        }

        [HttpPost]
        [ProducesResponseType(typeof(EnergiaPrediction), 200)]
        [ProducesResponseType(400)]
        public IActionResult Predict([FromBody] EnergiaInputModel input)
        {
            // Criar um objeto de entrada para previsão
            var inputData = new EnergiaData
            {
                ConsumoKWH = input.ConsumoKWH,
                GeracaoKWH = input.GeracaoKWH
            };

            // Criar um motor de previsão
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<EnergiaData, EnergiaPrediction>(_model);

            // Fazer a previsão
            var prediction = predictionEngine.Predict(inputData);

            return Ok(new { ProximoConsumoKWHPrevisto = prediction.ProximoConsumoKWH });
        }
    }

    // Definição das classes EnergiaData e EnergiaPrediction
    public class EnergiaData
    {
        public float ConsumoKWH { get; set; }
        public float GeracaoKWH { get; set; }
        public float ProximoConsumoKWH { get; set; } // O valor que queremos prever
    }

    public class EnergiaPrediction
    {
        [ColumnName("Score")]
        public float ProximoConsumoKWH { get; set; }
    }

    public class EnergiaInputModel
    {
        public float ConsumoKWH { get; set; }
        public float GeracaoKWH { get; set; }
    }
}