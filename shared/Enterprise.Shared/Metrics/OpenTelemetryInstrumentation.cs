using System.Collections.Concurrent;
using System.Diagnostics.Metrics;
using System.Text.RegularExpressions;

namespace Enterprise.Shared.Metrics;

public interface IOpenTelemetryInstrumentation
{
    Counter<T> GetCounterByName<T>(string counterName) where T : struct;

    ObservableGauge<T> GetObservableGaugeByName<T>(
        string name,
        Func<Measurement<T>> measurement) where T : struct;
}

public partial class OpenTelemetryInstrumentation : IOpenTelemetryInstrumentation
{
    private readonly ConcurrentDictionary<string, Instrument> _instruments = new();

    private readonly Meter _meter = new(
        MeterProviderNaming.UnityHubMeterProviderName,
        MeterProviderNaming.UnityHubMeterProviderVersion);

    public Counter<T> GetCounterByName<T>(string counterName) where T : struct
    {
        var result = GetOrCreate(counterName, () => _meter.CreateCounter<T>(counterName));

        return (Counter<T>)result;
    }

    public ObservableGauge<T> GetObservableGaugeByName<T>(
        string gaugeName,
        Func<Measurement<T>> measurement) where T : struct
    {
        var result = GetOrCreate(gaugeName,
            () => _meter.CreateObservableGauge(gaugeName, measurement));

        return (ObservableGauge<T>)result;
    }

    private Instrument GetOrCreate(string key, Func<Instrument> method)
    {
        CheckIsInstrumentNameValid(key);

        if (_instruments.TryGetValue(key, out var result))
        {
            return result;
        }

        result = method();
        _instruments.TryAdd(key, result);

        return result;
    }

    private static void CheckIsInstrumentNameValid(string instrumentName)
    {
        var regex = InstrumentNameValidationRegex();
        if (!regex.IsMatch(instrumentName))
        {
            throw new ArgumentException(nameof(instrumentName));
        }
    }

    [GeneratedRegex("((([A-z]+)([\\.])([A-z]+))+)$")]
    private static partial Regex InstrumentNameValidationRegex();
}
