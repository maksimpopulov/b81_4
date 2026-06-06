﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using BugPro;

namespace BugTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void NewBug_ShouldStartInCreatedState()
        {
            var bug = new Bug();
            Assert.AreEqual(BugState.New, bug.State, "Initial state must be New");
        }

        [TestMethod]
        public void Triage_ShouldBeReachableFromNew()
        {
            var bug = new Bug();
            bug.Fire(BugTrigger.StartTriage);
            Assert.AreEqual(BugState.Triage, bug.State, "After StartTriage state should be Triage");
        }

        [TestMethod]
        public void NeedInfo_ShouldTransitionFromTriage()
        {
            var bug = new Bug();
            bug.Fire(BugTrigger.StartTriage);
            bug.Fire(BugTrigger.NeedMoreInfo);
            Assert.AreEqual(BugState.NeedInfo, bug.State);
        }

        [TestMethod]
        public void NeedInfo_CanGoBackToTriage()
        {
            var bug = new Bug();
            bug.Fire(BugTrigger.StartTriage);
            bug.Fire(BugTrigger.NeedMoreInfo);
            bug.Fire(BugTrigger.ReturnToTriage);
            Assert.AreEqual(BugState.Triage, bug.State);
        }

        [TestMethod]
        public void AssignToDev_MovesToInProgress()
        {
            var bug = new Bug();
            bug.Fire(BugTrigger.StartTriage);
            bug.Fire(BugTrigger.AssignToDev);
            Assert.AreEqual(BugState.InProgress, bug.State);
        }

        [TestMethod]
        public void Fix_ChangesStateFromInProgressToFixed()
        {
            var bug = new Bug();
            bug.Fire(BugTrigger.StartTriage);
            bug.Fire(BugTrigger.AssignToDev);
            bug.Fire(BugTrigger.Fix);
            Assert.AreEqual(BugState.Fixed, bug.State);
        }

        [TestMethod]
        public void Verify_ClosesFixedBug()
        {
            var bug = new Bug();
            bug.Fire(BugTrigger.StartTriage);
            bug.Fire(BugTrigger.AssignToDev);
            bug.Fire(BugTrigger.Fix);
            bug.Fire(BugTrigger.Verify);
            Assert.AreEqual(BugState.Closed, bug.State);
        }

        [TestMethod]
        public void Reopen_ReturnsToReopenedState()
        {
            var bug = new Bug();
            bug.Fire(BugTrigger.StartTriage);
            bug.Fire(BugTrigger.AssignToDev);
            bug.Fire(BugTrigger.Fix);
            bug.Fire(BugTrigger.Reopen);
            Assert.AreEqual(BugState.Reopened, bug.State);
        }

        [TestMethod]
        public void ReopenedBug_CanBeAssignedAgain()
        {
            var bug = new Bug();
            bug.Fire(BugTrigger.StartTriage);
            bug.Fire(BugTrigger.AssignToDev);
            bug.Fire(BugTrigger.Fix);
            bug.Fire(BugTrigger.Reopen);
            bug.Fire(BugTrigger.AssignToDev);
            Assert.AreEqual(BugState.InProgress, bug.State);
        }

        [TestMethod]
        public void Triage_MarkingNotABug_LeadsToNotABugState()
        {
            var bug = new Bug();
            bug.Fire(BugTrigger.StartTriage);
            bug.Fire(BugTrigger.MarkNotABug);
            Assert.AreEqual(BugState.NotABug, bug.State);
        }

        [TestMethod]
        public void NotABug_CanBeClosed()
        {
            var bug = new Bug();
            bug.Fire(BugTrigger.StartTriage);
            bug.Fire(BugTrigger.MarkNotABug);
            bug.Fire(BugTrigger.Close);
            Assert.AreEqual(BugState.Closed, bug.State);
        }

        [TestMethod]
        public void Duplicate_MarkingWorks()
        {
            var bug = new Bug();
            bug.Fire(BugTrigger.StartTriage);
            bug.Fire(BugTrigger.MarkDuplicate);
            Assert.AreEqual(BugState.Duplicate, bug.State);
        }

        [TestMethod]
        public void CannotReproduce_StateReachable()
        {
            var bug = new Bug();
            bug.Fire(BugTrigger.StartTriage);
            bug.Fire(BugTrigger.MarkCannotReproduce);
            Assert.AreEqual(BugState.CannotReproduce, bug.State);
        }
    }
}
