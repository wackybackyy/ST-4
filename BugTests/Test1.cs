using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stateless;
using BugPro;

namespace BugTests;

[TestClass]
public class UnitTest1 {
    private Bug bug = null!;

    [TestInitialize]
    public void Setup() {
        bug = new Bug();
    }

    [TestMethod]
    public void InitialState_ShouldBeNew() {
        Assert.AreEqual(Bug.State.New, bug.CurrentState);
    }

    [TestMethod]
    public void New_To_Triaged() {
        bug.Fire(Bug.Trigger.Triaging);
        Assert.AreEqual(Bug.State.Triaged, bug.CurrentState);
    }

    [TestMethod]
    public void Triaged_To_NeedInfo() {
        bug.Fire(Bug.Trigger.Triaging);
        bug.Fire(Bug.Trigger.RequestInfo);
        Assert.AreEqual(Bug.State.NeedInfo, bug.CurrentState);
    }

    [TestMethod]
    public void NeedInfo_To_Triaged() {
        bug.Fire(Bug.Trigger.Triaging);
        bug.Fire(Bug.Trigger.RequestInfo);
        bug.Fire(Bug.Trigger.ProvideInfo);
        Assert.AreEqual(Bug.State.Triaged, bug.CurrentState);
    }

    [TestMethod]
    public void Triaged_To_InProgress() {
        bug.Fire(Bug.Trigger.Triaging);
        bug.Fire(Bug.Trigger.StartFix);
        Assert.AreEqual(Bug.State.InProgress, bug.CurrentState);
    }

    [TestMethod]
    public void InProgress_To_Resolved() {
        bug.Fire(Bug.Trigger.Triaging);
        bug.Fire(Bug.Trigger.StartFix);
        bug.Fire(Bug.Trigger.Resolve);
        Assert.AreEqual(Bug.State.Resolved, bug.CurrentState);
    }

    [TestMethod]
    public void Resolved_To_Closed() {
        bug.Fire(Bug.Trigger.Triaging);
        bug.Fire(Bug.Trigger.StartFix);
        bug.Fire(Bug.Trigger.Resolve);
        bug.Fire(Bug.Trigger.Close);
        Assert.AreEqual(Bug.State.Closed, bug.CurrentState);
    }

    [TestMethod]
    public void Resolved_To_InProgress_ByReopen() {
        bug.Fire(Bug.Trigger.Triaging);
        bug.Fire(Bug.Trigger.StartFix);
        bug.Fire(Bug.Trigger.Resolve);
        bug.Fire(Bug.Trigger.Reopen);
        Assert.AreEqual(Bug.State.InProgress, bug.CurrentState);
    }

    [TestMethod]
    public void Closed_To_InProgress_ByReopen() {
        bug.Fire(Bug.Trigger.Triaging);
        bug.Fire(Bug.Trigger.StartFix);
        bug.Fire(Bug.Trigger.Resolve);
        bug.Fire(Bug.Trigger.Close);
        bug.Fire(Bug.Trigger.Reopen);
        Assert.AreEqual(Bug.State.InProgress, bug.CurrentState);
    }

    [TestMethod]
    public void Triaged_To_Rejected() {
        bug.Fire(Bug.Trigger.Triaging);
        bug.Fire(Bug.Trigger.Reject);
        Assert.AreEqual(Bug.State.Rejected, bug.CurrentState);
    }

    [TestMethod]
    public void Rejected_To_Triaged_ByReopen() {
        bug.Fire(Bug.Trigger.Triaging);
        bug.Fire(Bug.Trigger.Reject);
        bug.Fire(Bug.Trigger.Reopen);
        Assert.AreEqual(Bug.State.Triaged, bug.CurrentState);
    }

    [TestMethod]
    public void Triaged_To_Duplicate() {
        bug.Fire(Bug.Trigger.Triaging);
        bug.Fire(Bug.Trigger.MarkDuplicate);
        Assert.AreEqual(Bug.State.Duplicate, bug.CurrentState);
    }

    [TestMethod]
    public void Duplicate_To_Triaged_ByReopen() {
        bug.Fire(Bug.Trigger.Triaging);
        bug.Fire(Bug.Trigger.MarkDuplicate);
        bug.Fire(Bug.Trigger.Reopen);
        Assert.AreEqual(Bug.State.Triaged, bug.CurrentState);
    }

    [TestMethod]
    public void Triaged_To_NotReproducible() {
        bug.Fire(Bug.Trigger.Triaging);
        bug.Fire(Bug.Trigger.MarkNotReproducible);
        Assert.AreEqual(Bug.State.NotReproducible, bug.CurrentState);
    }

    [TestMethod]
    public void NotReproducible_To_Triaged_ByReopen() {
        bug.Fire(Bug.Trigger.Triaging);
        bug.Fire(Bug.Trigger.MarkNotReproducible);
        bug.Fire(Bug.Trigger.Reopen);
        Assert.AreEqual(Bug.State.Triaged, bug.CurrentState);
    }

    [TestMethod]
    public void InProgress_To_NeedInfo() {
        bug.Fire(Bug.Trigger.Triaging);
        bug.Fire(Bug.Trigger.StartFix);
        bug.Fire(Bug.Trigger.RequestInfo);
        Assert.AreEqual(Bug.State.NeedInfo, bug.CurrentState);
    }

    [TestMethod]
    public void FullHappyPath_ShouldEndInClosed() {
        bug.Fire(Bug.Trigger.Triaging);
        bug.Fire(Bug.Trigger.StartFix);
        bug.Fire(Bug.Trigger.Resolve);
        bug.Fire(Bug.Trigger.Close);
        Assert.AreEqual(Bug.State.Closed, bug.CurrentState);
    }

    [TestMethod]
    public void NeedInfo_CanBeRejected() {
        bug.Fire(Bug.Trigger.Triaging);
        bug.Fire(Bug.Trigger.RequestInfo);
        bug.Fire(Bug.Trigger.Reject);
        Assert.AreEqual(Bug.State.Rejected, bug.CurrentState);
    }

    [TestMethod]
    public void ReopenAfterDuplicate_ThenStartFix() {
        bug.Fire(Bug.Trigger.Triaging);
        bug.Fire(Bug.Trigger.MarkDuplicate);
        bug.Fire(Bug.Trigger.Reopen);
        bug.Fire(Bug.Trigger.StartFix);
        Assert.AreEqual(Bug.State.InProgress, bug.CurrentState);
    }

    [TestMethod]
    public void ReopenAfterNotReproducible_ThenRequestInfo() {
        bug.Fire(Bug.Trigger.Triaging);
        bug.Fire(Bug.Trigger.MarkNotReproducible);
        bug.Fire(Bug.Trigger.Reopen);
        bug.Fire(Bug.Trigger.RequestInfo);
        Assert.AreEqual(Bug.State.NeedInfo, bug.CurrentState);
    }

    [TestMethod]
    public void New_CannotBeClosed_Directly() {
        Assert.ThrowsException<InvalidOperationException>(() => {
            bug.Fire(Bug.Trigger.Close);
        });
    }

    [TestMethod]
    public void New_CannotStartFix_Directly() {
        Assert.ThrowsException<InvalidOperationException>(() => {
            bug.Fire(Bug.Trigger.StartFix);
        });
    }

    [TestMethod]
    public void Triaged_CannotBeClosed_Directly() {
        bug.Fire(Bug.Trigger.Triaging);
        Assert.ThrowsException<InvalidOperationException>(() => {
            bug.Fire(Bug.Trigger.Close);
        });
    }

    [TestMethod]
    public void NeedInfo_CannotBeClosed_Directly() {
        bug.Fire(Bug.Trigger.Triaging);
        bug.Fire(Bug.Trigger.RequestInfo);
        Assert.ThrowsException<InvalidOperationException>(() => {
            bug.Fire(Bug.Trigger.Close);
        });
    }

    [TestMethod]
    public void InProgress_CannotBeClosed_Directly() {
        bug.Fire(Bug.Trigger.Triaging);
        bug.Fire(Bug.Trigger.StartFix);
        Assert.ThrowsException<InvalidOperationException>(() => {
            bug.Fire(Bug.Trigger.Close);
        });
    }

    [TestMethod]
    public void Closed_CannotBeClosedAgain() {
        bug.Fire(Bug.Trigger.Triaging);
        bug.Fire(Bug.Trigger.StartFix);
        bug.Fire(Bug.Trigger.Resolve);
        bug.Fire(Bug.Trigger.Close);

        Assert.ThrowsException<InvalidOperationException>(() => {
            bug.Fire(Bug.Trigger.Close);
        });
    }

    [TestMethod]
    public void Rejected_CannotResolve() {
        bug.Fire(Bug.Trigger.Triaging);
        bug.Fire(Bug.Trigger.Reject);

        Assert.ThrowsException<InvalidOperationException>(() => {
            bug.Fire(Bug.Trigger.Resolve);
        });
    }

    [TestMethod]
    public void Duplicate_CannotProvideInfo() {
        bug.Fire(Bug.Trigger.Triaging);
        bug.Fire(Bug.Trigger.MarkDuplicate);

        Assert.ThrowsException<InvalidOperationException>(() => {
            bug.Fire(Bug.Trigger.ProvideInfo);
        });
    }

    [TestMethod]
    public void NotReproducible_CannotStartFix() {
        bug.Fire(Bug.Trigger.Triaging);
        bug.Fire(Bug.Trigger.MarkNotReproducible);

        Assert.ThrowsException<InvalidOperationException>(() => {
            bug.Fire(Bug.Trigger.StartFix);
        });
    }

    [TestMethod]
    public void Resolved_CannotBeTriagedAgain() {
        bug.Fire(Bug.Trigger.Triaging);
        bug.Fire(Bug.Trigger.StartFix);
        bug.Fire(Bug.Trigger.Resolve);

        Assert.ThrowsException<InvalidOperationException>(() => {
            bug.Fire(Bug.Trigger.Triaging);
        });
    }
}
