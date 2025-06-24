using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteShadersUltimate.Demo
{
    public class Demo_Player : MonoBehaviour
    {
        public static Demo_Player instance;

        [Header("Sprites:")]
        public List<Sprite> idleSprites;
        public List<Sprite> runningSprites;
        public List<Sprite> hurtSprites;

        [Header("Other:")]
        public bool ignoreMaterials;

        //References:
        public SpriteRenderer spriteRenderer;
        public Rigidbody2D rig;

        //Runtime:
        float nextFrame;
        int currentIndex;
        List<Sprite> currentAnimation;
        Material originalMaterial;
        Material currentMaterial;
        Vector3 snapPosition;
        bool isShadow;

        void Start()
        {
            instance = this;

            //References:


            if (!ignoreMaterials)
            {
                currentMaterial = originalMaterial = spriteRenderer.material;
            }

            //Initialize:
            nextFrame = -1f;

           
        }

        void Update()
        {
          


            if (snapPosition != Vector3.zero)
            {
                //Snap to position:
                rig.velocity = Vector2.Lerp(rig.velocity, new Vector2(0, -1f), Time.deltaTime * 5f);
                if (Mathf.Abs(rig.velocity.x) < 1f && currentAnimation != hurtSprites)
                {
              
                }

                transform.position = Vector3.Lerp(transform.position, snapPosition, Time.deltaTime * 6f);

                if (Vector3.Distance(transform.position, snapPosition) < 0.1f) { snapPosition = Vector3.zero; Debug.Log("Teleportation Complete: " + transform.position); }
            }
           

            //Adjust Shadow Offset:
            if(isShadow)
            {
                MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                spriteRenderer.GetPropertyBlock(mpb);
                Vector2 offset = currentMaterial.GetVector("_ShadowOffset");
                if(transform.eulerAngles.y > 90)
                {
                    offset.x = -offset.x;
                }
                mpb.SetVector("_ShadowOffset", offset);
                spriteRenderer.SetPropertyBlock(mpb);
            }
        }

        public void GetHurt(Vector2 velocity)
        {
            rig.velocity = velocity;
            transform.eulerAngles = new Vector3(0, velocity.x > 0 ? 180 : 0, 0);
       
        }

     

        public void ApplyMaterial(Material material)
        {
            spriteRenderer.material = currentMaterial = material;

            isShadow = material.name.StartsWith("SSU_Demo_Shadow");
        }

        public void SnapPosition(Vector3 newPosition)
        {
            snapPosition = newPosition;
        }

        public void ResetPosition()
        {
            transform.position = new Vector3(6f, -2.645f, 0);
            transform.eulerAngles = new Vector3(0, 180, 0);
        }

        public void ResetMaterial()
        {
            spriteRenderer.material = originalMaterial;
            currentMaterial = originalMaterial;
            isShadow = false;
        }
    }
}
