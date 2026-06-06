﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using BugPro;

namespace BugTests;

[TestClass]
public class BugWorkflowTests
{
    [TestMethod]
    public void Test_InitialState_ShouldBeOpen()
    {
        var bug = new Bug();
        Assert.AreEqual(BugState.Open, bug.State);
    }

    [TestMethod]
    public void Test_Assign_ShouldChangeStateToAssigned()
    {
        var bug = new Bug();
        bug.Assign("TestDeveloper");
        Assert.AreEqual(BugState.Assigned, bug.State);
    }

    [TestMethod]
    public void Test_Assign_ShouldStoreAssigneeName()
    {
        var bug = new Bug();
        string expectedAssignee = "John Developer";
        bug.Assign(expectedAssignee);
        Assert.AreEqual(expectedAssignee, bug.AssignedTo);
    }

    [TestMethod]
    public void Test_StartWork_AfterAssign_ShouldChangeToInProgress()
    {
        var bug = new Bug();
        bug.Assign("Developer");
        bug.StartWork();
        Assert.AreEqual(BugState.InProgress, bug.State);
    }

    [TestMethod]
    public void Test_Fix_AfterStartWork_ShouldChangeToFixed()
    {
        var bug = new Bug();
        bug.Assign("Developer");
        bug.StartWork();
        bug.Fix();
        Assert.AreEqual(BugState.Fixed, bug.State);
    }

    [TestMethod]
    public void Test_Verify_AfterFix_ShouldChangeToVerified()
    {
        var bug = new Bug();
        bug.Assign("Developer");
        bug.StartWork();
        bug.Fix();
        bug.Verify();
        Assert.AreEqual(BugState.Verified, bug.State);
    }

    [TestMethod]
    public void Test_Close_AfterVerify_ShouldChangeToClosed()
    {
        var bug = new Bug();
        bug.Assign("Developer");
        bug.StartWork();
        bug.Fix();
        bug.Verify();
        bug.Close();
        Assert.AreEqual(BugState.Closed, bug.State);
    }

    [TestMethod]
    public void Test_Reopen_FromClosed_ShouldChangeToReopened()
    {
        var bug = new Bug();
        bug.Assign("Developer");
        bug.StartWork();
        bug.Fix();
        bug.Verify();
        bug.Close();
        bug.Reopen();
        Assert.AreEqual(BugState.Reopened, bug.State);
    }

    [TestMethod]
    public void Test_Reject_FromOpen_ShouldChangeToRejected()
    {
        var bug = new Bug();
        bug.Reject();
        Assert.AreEqual(BugState.Rejected, bug.State);
    }

    [TestMethod]
    public void Test_Reopen_FromRejected_ShouldChangeToReopened()
    {
        var bug = new Bug();
        bug.Reject();
        bug.Reopen();
        Assert.AreEqual(BugState.Reopened, bug.State);
    }

    [TestMethod]
    public void Test_CompleteWorkflow_ShouldEndInClosedState()
    {
        var bug = new Bug();
        bug.Assign("Developer");
        bug.StartWork();
        bug.Fix();
        bug.Verify();
        bug.Close();
        Assert.AreEqual(BugState.Closed, bug.State);
    }

    [TestMethod]
    public void Test_Reassign_FromAssigned_ShouldStayAssigned()
    {
        var bug = new Bug();
        bug.Assign("Dev1");
        bug.Assign("Dev2");
        Assert.AreEqual(BugState.Assigned, bug.State);
    }

    [TestMethod]
    public void Test_CannotStartWork_WithoutAssignment()
    {
        var bug = new Bug();
        Assert.ThrowsException<InvalidOperationException>(() => bug.StartWork());
    }

    [TestMethod]
    public void Test_CannotFix_WithoutStartingWork()
    {
        var bug = new Bug();
        bug.Assign("Developer");
        Assert.ThrowsException<InvalidOperationException>(() => bug.Fix());
    }

    [TestMethod]
    public void Test_MultipleReopenCloseCycles_ShouldWork()
    {
        var bug = new Bug();
        
        bug.Assign("Dev1");
        bug.StartWork();
        bug.Fix();
        bug.Verify();
        bug.Close();
        Assert.AreEqual(BugState.Closed, bug.State);
        
        bug.Reopen();
        Assert.AreEqual(BugState.Reopened, bug.State);
        
        bug.Assign("Dev2");
        bug.StartWork();
        bug.Fix();
        bug.Verify();
        bug.Close();
        
        Assert.AreEqual(BugState.Closed, bug.State);
    }
}
