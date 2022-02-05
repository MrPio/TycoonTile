using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class LogResourceScript : MonoBehaviour
    {
        public Circle circle;
        public float alpha;
        [SerializeField] public float scaleFactor = 1.2f;

        private void Start()
        {
            alpha = alpha / 2 + Mathf.PI / 4f;
            GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(alpha), Mathf.Sin(alpha)) * 6f;
            GetComponent<Rigidbody2D>().AddTorque(Random.Range(-30,30));
            StartCoroutine(Fall());
        }
        private IEnumerator Fall()
        {
            var rigidBody = GetComponent<Rigidbody2D>();
            var time = 0f;
            while (time < 0.38f)
            {
                rigidBody.AddForce(Vector2.down * 14f);
                yield return new WaitForSeconds(0.01f);
                time += 0.01f;
            }

            while (rigidBody.velocity.y < -2f) 
            {
                rigidBody.AddForce(Vector2.up * 10f);
                yield return new WaitForSeconds(0.01f);
            }
            rigidBody.freezeRotation = true;
            rigidBody.velocity = Vector2.zero;
            rigidBody.Sleep();
        }

        private void OnMouseEnter()
        {
            transform.localScale *= scaleFactor;
            transform.GetChild(0).gameObject.SetActive(true);
        }

        private void OnMouseExit()
        {
            transform.localScale /= scaleFactor;
            transform.GetChild(0).gameObject.SetActive(false);
        }

        private void OnMouseUp()
        {
            GameObject.Find("Main Camera").GetComponent<AudioSource>()
                .PlayOneShot(Resources.Load("Sounds/badge")as AudioClip);
            
            var res = Instantiate(Resources.Load("resource_earned") as GameObject, GameObject.Find("Canvas").transform);
            res.transform.position = transform.position+Vector3.up;
            var rigidBody = res.GetComponent<Rigidbody2D>();
            rigidBody.velocity=Vector2.up*3f;
            
            
            Circle circle = new Circle(0.2f, transform.position);
            float alpha = 0;
            for (int i = 0; i < 8; ++i)
            {
                var circleGo = Instantiate(Resources.Load("circle") as GameObject);
                circleGo.transform.SetPositionAndRotation(circle.getPointFromAngle(alpha),
                    Quaternion.Euler(0, 0, Mathf.Rad2Deg * alpha));
                circleGo.transform.localScale = new Vector2(0.12f, 0.12f);
                circleGo.GetComponent<SpriteRenderer>().sortingLayerName = "Grid";
                circleGo.GetComponent<SpriteRenderer>().sortingOrder = 99;
                circleGo.GetComponent<SpriteRenderer>().color = Color.white;

                var direction = circle.getPointFromAngle(alpha) - circle.center;
                circleGo.GetComponent<Rigidbody2D>().velocity = direction * CircleScript.Velocity;
                circleGo.GetComponent<Rigidbody2D>().AddForce(-direction * 1600f);

                alpha += 2 * Mathf.PI / 8;
            }
            Destroy(gameObject);
        }
    }
}