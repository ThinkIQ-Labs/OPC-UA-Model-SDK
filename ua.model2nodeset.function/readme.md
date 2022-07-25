# How to Use the Azure Function to Generate a Nodeset XML from a Model XML

``` C#
async Task<string> GetNodeSetXml() {
    var modelxml = md.GenerateModelXML();
    var jobject = new JObject();
    jobject["xml"] = modelxml;
    var json = Newtonsoft.Json.JsonConvert.SerializeObject(jobject);

    HttpClient aClient = new HttpClient();
    aClient.BaseAddress = new Uri($"https://{FunctionNamespace}.azurewebsites.net");
    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");

    var response = await aClient.PostAsync($"api/{FunctionName}?code={FunctionKey}", httpContent);
    var result = await response.Content.ReadAsStringAsync();

    return result;        
}
```