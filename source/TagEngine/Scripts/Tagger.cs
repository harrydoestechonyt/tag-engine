namespace HarryDoesTechStudios.TagEngine
{
    using Photon.Pun;
    using UnityEngine;

    public class Tagger : MonoBehaviour
    {
        public string HitboxTag;
        [HideInInspector] public bool isTag;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == HitboxTag)
            {
                PhotonView tagTarget = other.GetComponent<PhotonView>();
                if(tagTarget != null && tagTarget.Owner != PhotonNetwork.LocalPlayer)
                {
                    TagHitbox hitbox = tagTarget.GetComponent<TagHitbox>();
                    if (hitbox != null && !hitbox.isTag && isTag)
                    {
                        tagTarget.RPC(nameof(hitbox.OnHit), RpcTarget.AllBuffered, PlayerPrefs.GetString("Username", "player"));
                    }
                }
            }
        }
    }

}