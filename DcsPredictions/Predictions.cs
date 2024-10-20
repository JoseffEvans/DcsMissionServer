using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace DcsDataManagment {
    public class Predictions {

        const string Script = "./DcsPredictions.py";

        public async static Task<DcsPredictions> Predict(string data) =>
            await Predict(
                JsonConvert.DeserializeObject<DcsData>(data)
                    ?? throw new Exception($"Data converted to null string, data: {data}")
            );

        public async static Task<DcsPredictions> Predict(DcsData data) {
            var trainingData = 
                from unit in data.Units
                select new int[] { unit.X, unit.Y, unit.Type };

            try {
                var startInfo = new ProcessStartInfo {
                    FileName = "python3",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                startInfo.ArgumentList.Add(Script);
                startInfo.ArgumentList.Add(JsonConvert.SerializeObject(trainingData));

                using var scriptProcess = new Process {
                    StartInfo = startInfo
                };

                string output = await scriptProcess.StandardOutput.ReadToEndAsync();
                string error = await scriptProcess.StandardError.ReadToEndAsync();

                await scriptProcess.WaitForExitAsync();

                if(scriptProcess.ExitCode != 0 || !string.IsNullOrEmpty(error)) 
                    throw new Exception($"Prediction script failed to run. Exited with code {scriptProcess.ExitCode}. Error output: {error}");
                
                return JsonConvert.DeserializeObject<DcsPredictions>(output)
                    ?? throw new Exception("Python script did not write to output");
            }catch(Exception ex) {
                throw new Exception("", ex); 
            }
        }
    }
}
