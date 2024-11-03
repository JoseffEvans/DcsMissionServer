using Newtonsoft.Json;
using System.Diagnostics;

namespace DcsPredictions {
    public class Prediction {

        const string Script = "./DcsPredictions.py";

        public async static Task<DcsPredictions> Predict(string data) =>
            await Predict(
                JsonConvert.DeserializeObject<DcsData>(data)
                    ?? throw new Exception($"Data converted to null string, data: {data}")
            );

        public async static Task<DcsPredictions> Predict(DcsData data) {
            var trainingData = 
                from unit in data.Units
                select new object[] { unit.X, unit.Y, unit.Team };

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

                scriptProcess.Start();

                string output = await scriptProcess.StandardOutput.ReadToEndAsync();
                string error = await scriptProcess.StandardError.ReadToEndAsync();

                await scriptProcess.WaitForExitAsync();

                if(scriptProcess.ExitCode != 0 || !string.IsNullOrEmpty(error)) 
                    throw new Exception($"Prediction script failed to run. Exited with code {scriptProcess.ExitCode}.\nStd output: {output}\nError output: {error}");

                File.WriteAllText($"./TestOutput/pyScriptOutput{DateTime.Now:ddMMyyyy_hhmmss}.json", output);
                return JsonConvert.DeserializeObject<DcsPredictions>(output)
                    ?? throw new Exception("Python script did not write to output");
            }catch(Exception ex) {
                throw new Exception("An error occurred running prediction python script", ex); 
            }
        }
    }
}
