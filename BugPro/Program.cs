using Stateless;
namespace BugPro
{
public enum BugState
{
    New,
    Triaging,
    NoTime,
    SeparateSolution,
    OtherProduct,
    NeedMoreInfo,
    Fixing,
    ProblemSolved,
    Closed,
    Reopened
}

// Действия (триггеры)
public enum BugTrigger
{
    StartTriaging,
    NoTimeNow,
    SeparateSolutionReq,
    OtherProductIssue,
    RequestMoreInfo,
    InfoProvided,
    StartFix,
    MarkSolved,
    ConfirmSolved,
    NotSolved,
    Close
}

public class Bug
{
    private readonly StateMachine<BugState, BugTrigger> _machine;
    private StateMachine<BugState, BugTrigger>.TriggerWithParameters<string>? _requestInfoTrigger;

    public BugState State => _machine.State;

    public event Action<BugState, BugState>? StateChanged;

    public Bug()
    {
        _machine = new StateMachine<BugState, BugTrigger>(BugState.New);
        ConfigureMachine();
    }

    private void ConfigureMachine()
    {
        _machine.Configure(BugState.New)
            .Permit(BugTrigger.StartTriaging, BugState.Triaging);
        _machine.Configure(BugState.Triaging)
            .Permit(BugTrigger.NoTimeNow, BugState.NoTime)
            .Permit(BugTrigger.SeparateSolutionReq, BugState.SeparateSolution)
            .Permit(BugTrigger.OtherProductIssue, BugState.OtherProduct)
            .Permit(BugTrigger.RequestMoreInfo, BugState.NeedMoreInfo)
            .Permit(BugTrigger.StartFix, BugState.Fixing);
        _machine.Configure(BugState.NoTime)
            .Permit(BugTrigger.StartTriaging, BugState.Triaging);
        _machine.Configure(BugState.SeparateSolution)
            .Permit(BugTrigger.MarkSolved, BugState.ProblemSolved);
        _machine.Configure(BugState.OtherProduct)
            .Permit(BugTrigger.Close, BugState.Closed);
        _machine.Configure(BugState.NeedMoreInfo)
            .Permit(BugTrigger.InfoProvided, BugState.Triaging)
            .Permit(BugTrigger.Close, BugState.Closed);
        _machine.Configure(BugState.Fixing)
            .Permit(BugTrigger.MarkSolved, BugState.ProblemSolved);
        _machine.Configure(BugState.ProblemSolved)
            .Permit(BugTrigger.ConfirmSolved, BugState.Closed)
            .Permit(BugTrigger.NotSolved, BugState.Reopened);
        _machine.Configure(BugState.Reopened)
            .Permit(BugTrigger.StartTriaging, BugState.Triaging)
            .Permit(BugTrigger.StartFix, BugState.Fixing);
        _machine.Configure(BugState.Closed)
            .PermitReentry(BugTrigger.Close);

        _machine.OnTransitioned(transition =>
        {
            StateChanged?.Invoke(transition.Source, transition.Destination);
        });
    }

    public void Fire(BugTrigger trigger)
    {
        if (_machine.CanFire(trigger))
        {
            _machine.Fire(trigger);
        }
        else
        {
            throw new InvalidOperationException($"Cannot fire {trigger} in state {State}");
        }
    }

    public bool CanFire(BugTrigger trigger) => _machine.CanFire(trigger);
}

public class Program
{
    public static void Main()
    {
        var bug = new Bug();

        Console.WriteLine("=== Демонстрация Workflow бага ===\n");

        bug.StateChanged += (from, to) =>
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Баг перешел из {from} в {to}");
        };

        Console.WriteLine($"Текущее состояние: {bug.State}");

        bug.Fire(BugTrigger.StartTriaging);
        Console.WriteLine($"После StartTriaging: {bug.State}");

        bug.Fire(BugTrigger.RequestMoreInfo);
        Console.WriteLine($"После RequestMoreInfo: {bug.State}");

        bug.Fire(BugTrigger.InfoProvided);
        Console.WriteLine($"После InfoProvided: {bug.State}");

        bug.Fire(BugTrigger.StartFix);
        Console.WriteLine($"После StartFix: {bug.State}");

        bug.Fire(BugTrigger.MarkSolved);
        Console.WriteLine($"После MarkSolved: {bug.State}");

        bug.Fire(BugTrigger.ConfirmSolved);
        Console.WriteLine($"После ConfirmSolved: {bug.State}");

        Console.WriteLine("\n=== Демонстрация ветки 'Нет времени сейчас' ===");

        var bug2 = new Bug();
        bug2.StateChanged += (from, to) => Console.WriteLine($"  {from} -> {to}");

        bug2.Fire(BugTrigger.StartTriaging);
        bug2.Fire(BugTrigger.NoTimeNow);
        bug2.Fire(BugTrigger.StartTriaging);

        Console.WriteLine("\n=== Попытка недопустимого перехода ===");
        try
        {
            bug.Fire(BugTrigger.NotSolved);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Ошибка (ожидаемо): {ex.Message}");
        }

        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.ReadKey();
    }
}
}
