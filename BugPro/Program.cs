using Stateless;

namespace BugPro;

public enum BugState
{
    Open,
    Assigned,
    InProgress,
    Fixed,
    Verified,
    Closed,
    Reopened,
    Rejected
}

public enum BugTrigger
{
    Assign,
    StartWork,
    Fix,
    Verify,
    Close,
    Reopen,
    Reject,
    Reassign
}

public class Bug
{
    private readonly StateMachine<BugState, BugTrigger> _machine;
    private readonly StateMachine<BugState, BugTrigger>.TriggerWithParameters<string> _assignTrigger;
    
    private BugState _state;
    public BugState State => _state;
    public string? AssignedTo { get; private set; }

    public Bug()
    {
        _state = BugState.Open;
        
        _machine = new StateMachine<BugState, BugTrigger>(
            () => _state,
            state => _state = state,
            () => BugState.Open);

        _assignTrigger = _machine.SetTriggerParameters<string>(BugTrigger.Assign);

        ConfigureMachine();
    }

    private void ConfigureMachine()
    {
        _machine.Configure(BugState.Open)
            .Permit(BugTrigger.Assign, BugState.Assigned)
            .Permit(BugTrigger.Reject, BugState.Rejected);

        _machine.Configure(BugState.Assigned)
            .Permit(BugTrigger.StartWork, BugState.InProgress)
            .PermitReentrant(BugTrigger.Reassign) // Используем PermitReentrant для повторного входа
            .OnEntryFrom(_assignTrigger, assignee => AssignedTo = assignee); // Правильный синтаксис

        _machine.Configure(BugState.InProgress)
            .Permit(BugTrigger.Fix, BugState.Fixed)
            .Permit(BugTrigger.Reject, BugState.Rejected);

        _machine.Configure(BugState.Fixed)
            .Permit(BugTrigger.Verify, BugState.Verified)
            .Permit(BugTrigger.Reopen, BugState.Reopened);

        _machine.Configure(BugState.Verified)
            .Permit(BugTrigger.Close, BugState.Closed)
            .Permit(BugTrigger.Reopen, BugState.Reopened);

        _machine.Configure(BugState.Closed)
            .Permit(BugTrigger.Reopen, BugState.Reopened);

        _machine.Configure(BugState.Reopened)
            .Permit(BugTrigger.Assign, BugState.Assigned)
            .Permit(BugTrigger.Reject, BugState.Rejected);

        _machine.Configure(BugState.Rejected)
            .Permit(BugTrigger.Reopen, BugState.Reopened);
    }

    public void Assign(string assignee)
    {
        _machine.Fire(_assignTrigger, assignee);
    }

    public void StartWork()
    {
        _machine.Fire(BugTrigger.StartWork);
    }

    public void Fix()
    {
        _machine.Fire(BugTrigger.Fix);
    }

    public void Verify()
    {
        _machine.Fire(BugTrigger.Verify);
    }

    public void Close()
    {
        _machine.Fire(BugTrigger.Close);
    }

    public void Reopen()
    {
        _machine.Fire(BugTrigger.Reopen);
    }

    public void Reject()
    {
        _machine.Fire(BugTrigger.Reject);
    }

    public void PrintState()
    {
        Console.WriteLine($"Current bug state: {State}");
        if (AssignedTo != null)
            Console.WriteLine($"Assigned to: {AssignedTo}");
    }
}

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Bug Workflow Demonstration ===\n");

        var bug = new Bug();
        
        bug.PrintState();
        
        Console.WriteLine("\n--- Assigning bug to developer ---");
        bug.Assign("John Doe");
        bug.PrintState();
        
        Console.WriteLine("\n--- Starting work ---");
        bug.StartWork();
        bug.PrintState();
        
        Console.WriteLine("\n--- Fixing bug ---");
        bug.Fix();
        bug.PrintState();
        
        Console.WriteLine("\n--- Verifying fix ---");
        bug.Verify();
        bug.PrintState();
        
        Console.WriteLine("\n--- Closing bug ---");
        bug.Close();
        bug.PrintState();
        
        Console.WriteLine("\n--- Reopening bug (found in production) ---");
        bug.Reopen();
        bug.PrintState();
        
        Console.WriteLine("\n--- Reassigning reopened bug ---");
        bug.Assign("Jane Smith");
        bug.PrintState();
        
        Console.WriteLine("\n--- Rejecting bug ---");
        bug.Reject();
        bug.PrintState();
        
        Console.WriteLine("\n--- Reopening rejected bug ---");
        bug.Reopen();
        bug.PrintState();
        
        Console.WriteLine("\n=== Workflow Complete ===");
    }
}
