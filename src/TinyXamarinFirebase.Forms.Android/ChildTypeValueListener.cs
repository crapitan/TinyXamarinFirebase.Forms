﻿using Firebase.Database;
using System.Collections.Generic;

namespace TinyXamarinFirebase.Forms.Droid
{
    public class ChildTypeValueListener<T> : ValueListenerBase, IChildEventListener
    {
        private readonly FirebaseChildEventDelegate<T> eventDelegate;
        private readonly FirebaseChildChangeEnum changeType;


        public ChildTypeValueListener(FirebaseChildEventDelegate<T> eventDelegate, FirebaseChildChangeEnum changeType)
        {
            this.eventDelegate = eventDelegate;
            this.changeType = changeType;
        }

        public void OnCancelled(DatabaseError error)
        {
            eventDelegate.OnError(new FirebaseException(error));
        }

        public void OnChildAdded(DataSnapshot snapshot, string previousChildName)
        {
            if (snapshot.Exists() && changeType == FirebaseChildChangeEnum.ChildAdded)
            {
                HandleChange(FirebaseChildChangeEnum.ChildAdded, snapshot, previousChildName);
            }
        }

        public void OnChildChanged(DataSnapshot snapshot, string previousChildName)
        {
            if (snapshot.Exists() && changeType == FirebaseChildChangeEnum.ChildChanged)
            {
                HandleChange(FirebaseChildChangeEnum.ChildChanged, snapshot, previousChildName);
            }
        }

        private void HandleChange(FirebaseChildChangeEnum type, DataSnapshot snapshot, string previousChildName)
        {
            FirebaseXamarinHelper.RunOnUIThread(() =>
            {
                var snapData = DataConverter.Convert<T>(snapshot.Value, GetObjectFromHandler(eventDelegate));
                eventDelegate.OnSnapshot(type, previousChildName, new KeyValuePair<string, T>(snapshot.Key, snapData));
                SetObjectFromHandler(eventDelegate, snapData);
            });
        }

        public void OnChildMoved(DataSnapshot snapshot, string previousChildName)
        {
            if (changeType == FirebaseChildChangeEnum.ChildMoved)
            {
                HandleChange(FirebaseChildChangeEnum.ChildMoved, snapshot, previousChildName);
            }
        }

        public void OnChildRemoved(DataSnapshot snapshot)
        {
            if (changeType == FirebaseChildChangeEnum.ChildRemoved)
            {
                HandleChange(FirebaseChildChangeEnum.ChildRemoved, snapshot, null);
            }
        }
    }
}