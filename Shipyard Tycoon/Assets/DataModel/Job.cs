using System.Collections;
using UnityEngine;
using System;

public class Job {
    //This class holds information for a queued up job, including things like building terrain, moving items etc.

    public Tile tile { get; protected set; }
    float jobTime = 1f;

    Action<Job> cbJobComplete;
    Action<Job> cbJobCancel;

    public Job ( Tile tile, Action <Job> cbJobComplete ,float jobTime = 1f) {
        this.tile = tile;
        this.cbJobComplete += cbJobComplete;
    }

    public void DoWork (float workTime) {
        jobTime -= workTime;

        if(jobTime <= 0) {
            if( cbJobComplete != null) {
                cbJobComplete(this);
            }
        }
    }

    public void CancelJob() {
        if(cbJobCancel != null) {
            cbJobCancel(this);
        }
    }

    public void RegisterJobCompleteCallback(Action<Job> callback) {
        cbJobComplete += callback;
    }

    public void UnregisterJobCompleteCallback(Action<Job> callback) {
        cbJobComplete -= callback;
    }

    public void RegisterJobCancelCallback(Action<Job> callback) {
        cbJobCancel += callback;
    }

    public void UnregisterJobCancelCallback(Action<Job> callback) {
        cbJobCancel -= callback;
    }
}
