namespace HarryDoesTechStudios.TagEngine
{
    using System.Collections;
    using System.Collections.Generic;
    using Photon.Pun;
    using Photon.VR;
    using UnityEngine;

    [RequireComponent(typeof(PhotonView))]
    public class TagManager : MonoBehaviour, IPunObservable
    {
        

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }

        [Header("Make sure you credit HarryDoesTech for the TagEngine,\nas required in the MIT License.")]
        
        public PhotonView ptView;

        [Header("Settings")]
        public float endTime;
        public int peopleNeeded;

        bool isMatster;
        TagHitbox[] hitboxes;
        int TaggedPeople;
        bool canStartNewRound = true;
        int randomStartPlayerNumber = 00000;
        TagHitbox newTaggedPlayer;
        int TotalPlayers;
        List<Tagger> taggingHands;

        public void CheckIfRoundShouldStart()
        {
            if (TaggedPeople == 0 && canStartNewRound && TotalPlayers >= peopleNeeded)
            {
                StartCoroutine(StartRound());
            }
        }

        public IEnumerator StartRound()
        {
            if (canStartNewRound)
            {
                if (randomStartPlayerNumber == 00000)
                {
                    randomStartPlayerNumber = Random.Range(0, hitboxes.Length);
                    newTaggedPlayer = hitboxes[randomStartPlayerNumber];
                }
                if (newTaggedPlayer != null)
                {
                    PhotonView taggedPlayer = newTaggedPlayer.GetComponent<PhotonView>();
                    if (taggedPlayer != null)
                    {
                        taggedPlayer.RPC(nameof(TagHitbox.OnHit), RpcTarget.AllBuffered, "the round starting");
                    }
                }
            }
            yield return null;
        }

        void SetTagStatus(bool state)
        {
            foreach (Tagger t in taggingHands)
            {
                t.enabled = state;
            }

            hitboxes = FindObjectsOfType<TagHitbox>();

            foreach (TagHitbox t in hitboxes)
            {
                t.enabled = state;
            }
        }

        void FixedUpdate()
        {
            isMatster = PhotonNetwork.IsMasterClient;
            if (isMatster)
            {
                TotalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
                CheckTaggedPlayers();
                CheckIfRoundShouldEnd();
                CheckIfRoundShouldStart();
            }
        }

        public void CheckIfRoundShouldEnd()
        {
            if (TotalPlayers <= TaggedPeople)
            {
                EndRound();
            }
        }

        void CheckTaggedPlayers()
        {
            TaggedPeople = 0;
            hitboxes = FindObjectsOfType<TagHitbox>();
            foreach (TagHitbox t in hitboxes)
            {
                if (t.isTag)
                {
                    TaggedPeople++;
                }
            }
        }

        public void EndRound()
        {
            randomStartPlayerNumber = 00000;
            foreach (TagHitbox t in hitboxes)
            {
                PhotonView hitboxPV = t.GetComponent<PhotonView>();
                if (hitboxPV != null)
                {
                    hitboxPV.RPC(nameof(TagHitbox.EndRound), RpcTarget.AllBuffered);
                }
            }
            StartCoroutine(EndRoundWait());
        }

        private IEnumerator EndRoundWait()
        {
            canStartNewRound = false;
            yield return new WaitForSeconds(endTime);
            canStartNewRound = true;
        }
    }
}