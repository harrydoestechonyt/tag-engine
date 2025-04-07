namespace HarryDoesTechStudios.TagEngine
{
    using System.Collections;
    using System.Collections.Generic;
    using Photon.Pun;
    using UnityEngine;
    using GorillaLocomotion;
    using System.Text;
    using UnityEngine.Networking;

    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(PhotonView))]
    public class TagHitbox : MonoBehaviour, IPunObservable
    {
        public PhotonView ptView;

        [Header("Audio Stuff")]
        public List<AudioClip> tagSounds;

        public AudioClip roundOver;

        [Header("Settings")]

        public bool useTagFreeze;
        public string GorillaPlayerReference;
        public float tagFreezeTime;

        public bool useSpeedBoost;
        public float jumpMultiplier;
        public float velLimit;
        public float maxJmpSpd;

        public Material untagged;

        [Header("If you use MeshRenderer")]
        public List<MeshRenderer> renderers;
        public bool useMeshRenderers;

        [Header("If you use SkinnedMeshRenderer")]
        public List<SkinnedMeshRenderer> skinnedRenderers;
        public bool useSkinnedMeshRenderers;

        [Header("Discord Webhook (optional)")]
        public bool sendTagLogToDiscordWebhook;
        public string webhookURL;

        Player gorillaPlayer;
        Tagger[] hands;
        [HideInInspector] public bool isTag;
        [HideInInspector] public AudioSource Player;
        float OmaxJmpSpd;
        float ojumpMultiplier;
        float oVelLimit;
        Color originalColour;

        void Start()
        {
            if (useMeshRenderers) {
                foreach (Renderer renderer in renderers) {
                    originalColour = renderer.material.color;
                }
            }

            if (useSkinnedMeshRenderers)
            {
                foreach (SkinnedMeshRenderer renderer in skinnedRenderers)
                {
                    originalColour = renderer.material.color;
                }
            }
            Player = this.GetComponent<AudioSource>();
            ptView = this.GetComponent<PhotonView>();
            hands = FindObjectsOfType<Tagger>();
            if (useTagFreeze || useSpeedBoost)
                gorillaPlayer = GameObject.Find(GorillaPlayerReference).GetComponent<Player>();
            if (useSpeedBoost)
            {
                OmaxJmpSpd = gorillaPlayer.velocityLimit;
                ojumpMultiplier = gorillaPlayer.jumpMultiplier;
                oVelLimit = gorillaPlayer.maxJumpSpeed;
            }
        }

        void Update()
        {
            if (!isTag)
            {
                if (useMeshRenderers)
                {
                    foreach (Renderer renderer in renderers)
                    {
                        originalColour = renderer.material.color;
                    }
                }

                if (useSkinnedMeshRenderers)
                {
                    foreach (SkinnedMeshRenderer renderer in skinnedRenderers)
                    {
                        originalColour = renderer.material.color;
                    }
                }
            }
        }

        [PunRPC]
        public void EndRound()
        {
            Player.clip = roundOver;
            Player.Play();
            isTag = false;
            if (useMeshRenderers)
            {
                foreach (MeshRenderer renderer in renderers) { 
                    renderer.material.color = originalColour;
                }
            }else if (useSkinnedMeshRenderers)
            {
                foreach (SkinnedMeshRenderer renderer in skinnedRenderers)
                {
                    renderer.material.color = originalColour;
                }
            }
            foreach (Tagger t in hands)
            {
                if (t != null)
                {
                    t.isTag = false;
                }
            }
            if (useSpeedBoost)
            {
                RemoveSpeedBoost();
            }
        }

        [PunRPC]
        public void OnHit(string tagger)
        {
            if (!isTag)
            {
                if (tagSounds != null)
                {
                    int RIndex = Random.Range(0, tagSounds.Count);
                    Player.clip = tagSounds[RIndex];
                    Player.Play();
                }
                isTag = true;
                foreach (Tagger t in hands)
                {
                    if (t != null)
                    {
                        t.isTag = true;
                    }
                }

                if (useMeshRenderers)
                {
                    if (renderers != null)
                    {
                        foreach (MeshRenderer renderer in renderers)
                        {
                            if (renderer != null)
                            {
                                renderer.material.color = Color.red;
                            }
                        }
                    }
                }
                else if (useSkinnedMeshRenderers)
                {
                    if (renderers != null)
                    {
                        foreach (SkinnedMeshRenderer renderer in skinnedRenderers)
                        {
                            if (renderer != null)
                            {
                                renderer.material.color = Color.red;
                            }
                        }
                    }
                }

                if (useTagFreeze)
                    StartCoroutine(TagFreeze());

                if (useSpeedBoost)
                    AddSpeedBoost();

                if (sendTagLogToDiscordWebhook)
                    StartCoroutine(sendTagLog($"{tagger} just tagged {PhotonNetwork.LocalPlayer.NickName}!"));
            }

            IEnumerator sendTagLog(string msg)
            {
                string jsonPayload = "{\"content\": \"" + msg + "\"}";

                UnityWebRequest www = new UnityWebRequest(webhookURL, "POST");
                byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonPayload);
                www.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
                www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");

                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                    Debug.LogWarning($"Error logging tag: {www.error}");
            }
        }

        public void AddSpeedBoost()
        {
            gorillaPlayer.maxJumpSpeed = maxJmpSpd;
            gorillaPlayer.jumpMultiplier = jumpMultiplier;
            gorillaPlayer.velocityLimit = velLimit;
        }

        public void RemoveSpeedBoost()
        {
            gorillaPlayer.maxJumpSpeed = OmaxJmpSpd;
            gorillaPlayer.jumpMultiplier = ojumpMultiplier;
            gorillaPlayer.velocityLimit = oVelLimit;
        }

        public IEnumerator TagFreeze()
        {
            if (isTag)
            {
                gorillaPlayer.disableMovement = true;
                yield return new WaitForSeconds(tagFreezeTime);
                gorillaPlayer.disableMovement = false;
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
                stream.SendNext(isTag);
            else
            {
                isTag = (bool)stream.ReceiveNext();
                if (useMeshRenderers)
                    foreach (MeshRenderer renderer in renderers)
                    {
                        renderer.material = isTag ? new Material(Shader.Find("Standard")) : untagged;
                        renderer.material.color = isTag ? Color.red : originalColour;

                    }
                else if (useSkinnedMeshRenderers)
                    foreach (SkinnedMeshRenderer renderer in skinnedRenderers)
                    {
                        if(renderer.materials.Length > 1)
                        {
                            int idx = 0;
                            Material[] mats = renderer.materials;
                            foreach(Material mat in mats)
                            {
                                mats[idx] = isTag ? new Material(Shader.Find("Standard")) : untagged;
                                mats[idx].color = isTag ? Color.red : originalColour;
                                idx++;
                            }
                            renderer.materials = mats;
                        }
                        else
                        {
                            renderer.material = isTag ? new Material(Shader.Find("Standard")) : untagged;
                            renderer.material.color = isTag ? Color.red : originalColour;
                        }
                    }
            }
        }
    }
}
