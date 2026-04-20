using Stateless;

namespace BugPro;

public class Bug {
    public enum State {
        New,
        Triaged,
        NeedInfo,
        InProgress,
        Resolved,
        Closed,
        Rejected,
        Duplicate,
        NotReproducible
    }

    public enum Trigger {
        Triaging,
        RequestInfo,
        ProvideInfo,
        StartFix,
        Resolve,
        Close,
        Reopen,
        Reject,
        MarkDuplicate,
        MarkNotReproducible
    }

    private readonly StateMachine<State, Trigger> machine;

    public Bug() {
        machine = new StateMachine<State, Trigger>(State.New);

        machine.Configure(State.New)
            .Permit(Trigger.Triaging, State.Triaged);

        machine.Configure(State.Triaged)
            .Permit(Trigger.RequestInfo, State.NeedInfo)
            .Permit(Trigger.StartFix, State.InProgress)
            .Permit(Trigger.Reject, State.Rejected)
            .Permit(Trigger.MarkDuplicate, State.Duplicate)
            .Permit(Trigger.MarkNotReproducible, State.NotReproducible);

        machine.Configure(State.NeedInfo)
            .Permit(Trigger.ProvideInfo, State.Triaged)
            .Permit(Trigger.Reject, State.Rejected);

        machine.Configure(State.InProgress)
            .Permit(Trigger.Resolve, State.Resolved)
            .Permit(Trigger.RequestInfo, State.NeedInfo);

        machine.Configure(State.Resolved)
            .Permit(Trigger.Close, State.Closed)
            .Permit(Trigger.Reopen, State.InProgress);

        machine.Configure(State.Closed)
            .Permit(Trigger.Reopen, State.InProgress);

        machine.Configure(State.Rejected)
            .Permit(Trigger.Reopen, State.Triaged);

        machine.Configure(State.Duplicate)
            .Permit(Trigger.Reopen, State.Triaged);

        machine.Configure(State.NotReproducible)
            .Permit(Trigger.Reopen, State.Triaged);
    }

    public State CurrentState => machine.State;

    public void Fire(Trigger trigger) {
        machine.Fire(trigger);
    }
}

internal class Program {
    private static void Main(string[] args) {
        var bug = new Bug();

        Console.WriteLine($"Initial state: {bug.CurrentState}");
        bug.Fire(Bug.Trigger.Triaging);
        Console.WriteLine($"After triage: {bug.CurrentState}");
        bug.Fire(Bug.Trigger.StartFix);
        Console.WriteLine($"After start fix: {bug.CurrentState}");
        bug.Fire(Bug.Trigger.Resolve);
        Console.WriteLine($"After resolve: {bug.CurrentState}");
        bug.Fire(Bug.Trigger.Close);
        Console.WriteLine($"After close: {bug.CurrentState}");
    }
}
