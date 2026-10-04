using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

using lab2.Models;

namespace lab2
{
    public class CalculatorTrigger
    {
        // =========================
        // GET
        // =========================

        [Function("CalculatorGet")]
        public IActionResult Get(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "get",
                Route = "calculator/{number1}/{number2}/{operation}"
            )]
            HttpRequest req,
            double number1,
            double number2,
            string operation)
        {
            double result;

            switch (operation.ToLower())
            {
                case "add":
                    result = number1 + number2;
                    break;

                case "minus":
                    result = number1 - number2;
                    break;

                case "multiply":
                    result = number1 * number2;
                    break;

                case "div":
                    if (number2 == 0)
                    {
                        return new BadRequestObjectResult(
                            "Cannot divide by zero."
                        );
                    }

                    result = number1 / number2;
                    break;

                default:
                    return new BadRequestObjectResult(
                        "Unknown operation."
                    );
            }

            return new OkObjectResult(
                new CalculationResult
                {
                    Result = result,
                    Operation = operation
                }
            );
        }


        // =========================
        // POST
        // =========================

        [Function("CalculatorPost")]
        public async Task<IActionResult> Post(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "post",
                Route = "calculator"
            )]
            HttpRequest req)
        {
            string requestBody =
                await new StreamReader(req.Body).ReadToEndAsync();

            CalculationRequest? request;

            try
            {
                request = JsonSerializer.Deserialize<CalculationRequest>(
                    requestBody,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }
                );
            }
            catch
            {
                return new BadRequestObjectResult(
                    "Invalid JSON."
                );
            }

            if (request == null)
            {
                return new BadRequestObjectResult(
                    "Invalid request."
                );
            }

            double result;

            switch (request.Operator.ToLower())
            {
                case "add":
                    result = request.Number1 + request.Number2;
                    break;

                case "minus":
                    result = request.Number1 - request.Number2;
                    break;

                case "multiply":
                    result = request.Number1 * request.Number2;
                    break;

                case "div":
                    if (request.Number2 == 0)
                    {
                        return new BadRequestObjectResult(
                            "Cannot divide by zero."
                        );
                    }

                    result = request.Number1 / request.Number2;
                    break;

                default:
                    return new BadRequestObjectResult(
                        "Unknown operation."
                    );
            }

            return new OkObjectResult(
                new CalculationResult
                {
                    Result = result,
                    Operation = request.Operator
                }
            );
        }
    }
}