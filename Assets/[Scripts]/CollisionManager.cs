using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollisionManager : MonoBehaviour
{
    public CubeBehaviour[] cubes;
    public BulletBehaviour[] spheres;
    public CubeBulletBehaviour[] bullets;

    private static Vector3[] faces;

   
    // Update is called once per frame
    void Update()
    {
        spheres = FindObjectsOfType<BulletBehaviour>();
        bullets = FindObjectsOfType<CubeBulletBehaviour>();
        cubes = FindObjectsOfType<CubeBehaviour>();

        faces = new Vector3[]
        {
            Vector3.left, Vector3.right,
            Vector3.down, Vector3.up,
            Vector3.back , Vector3.forward
        };
        // check each AABB with every other AABB in the scene
        for (int i = 0; i < cubes.Length; i++)
        {
            for (int j = 0; j < cubes.Length; j++)
            {
                if (i != j)
                {
                    CheckAABBs(cubes[i], cubes[j]);
                }
            }
        }
        foreach (var sphere in spheres)
        {
            foreach (var cube in cubes)
            {
                if (cube.name != "Player")
                {
                    CheckSphereAABB(sphere, cube);
                }
            }
        }

        foreach (var bullet in bullets)
        {
            foreach (var cube in cubes)
            {
                if (cube.name != "Player")
                {
                    Reflect(bullet);
                }

            }
        }
    }
    
    public static void CheckSphereAABB(BulletBehaviour s, CubeBehaviour b)
    {
        // get box closest point to sphere center by clamping
        var x = Mathf.Max(b.min.x, Mathf.Min(s.transform.position.x, b.max.x));
        var y = Mathf.Max(b.min.y, Mathf.Min(s.transform.position.y, b.max.y));
        var z = Mathf.Max(b.min.z, Mathf.Min(s.transform.position.z, b.max.z));

        var distance = Math.Sqrt((x - s.transform.position.x) * (x - s.transform.position.x) +
                                 (y - s.transform.position.y) * (y - s.transform.position.y) +
                                 (z - s.transform.position.z) * (z - s.transform.position.z));

        if ((distance < s.radius) && (!s.isColliding))
        {
            // determine the distances between the contact extents
            float[] distances = {
                (b.max.x - s.transform.position.x),
                (s.transform.position.x - b.min.x),
                (b.max.y - s.transform.position.y),
                (s.transform.position.y - b.min.y),
                (b.max.z - s.transform.position.z),
                (s.transform.position.z - b.min.z)
            };

            float penetration = float.MaxValue;
            Vector3 face = Vector3.zero;

            // check each face to see if it is the one that connected
            for (int i = 0; i < 6; i++)
            {
                if (distances[i] < penetration)
                {
                    // determine the penetration distance
                    penetration = distances[i];
                    face = faces[i];
                }
            }

            s.penetration = penetration;
            s.collisionNormal = face;

            Reflect(s);
        }

    }
    
    // This helper function reflects the bullet when it hits an AABB face
    private static void Reflect(BulletBehaviour s)
    {
        if ((s.collisionNormal == Vector3.forward) || (s.collisionNormal == Vector3.back))
        {
            s.direction = new Vector3(s.direction.x, s.direction.y, -s.direction.z);
        }
        else if ((s.collisionNormal == Vector3.right) || (s.collisionNormal == Vector3.left))
        {
            s.direction = new Vector3(-s.direction.x, s.direction.y, s.direction.z);
        }
        else if ((s.collisionNormal == Vector3.up) || (s.collisionNormal == Vector3.down))
        {
            s.direction = new Vector3(s.direction.x, -s.direction.y, s.direction.z);
        }
    }

    public static void CheckAABBs(CubeBehaviour a, CubeBehaviour b)
    {
        Contact contactB = new Contact(b);
        float movementScalarA = 0.0f;
        float movementScalarB = 0.0f;
        if ((a.min.x <= b.max.x && a.max.x >= b.min.x) &&
            (a.min.y <= b.max.y && a.max.y >= b.min.y) &&
            (a.min.z <= b.max.z && a.max.z >= b.min.z))
        {
            // determine the distances between the contact extents
            float[] distances = {
                (b.max.x - a.min.x),
                (a.max.x - b.min.x),
                (b.max.y - a.min.y),
                (a.max.y - b.min.y),
                (b.max.z - a.min.z),
                (a.max.z - b.min.z)
            };

            float penetration = float.MaxValue;
            Vector3 face = Vector3.zero;

            // check each face to see if it is the one that connected
            for (int i = 0; i < 6; i++)
            {
                if (distances[i] < penetration)
                {
                    // determine the penetration distance
                    penetration = distances[i];
                    face = faces[i];
                }
            }
            
            // set the contact properties
            contactB.face = face;
            contactB.penetration = penetration;


            // check if contact does not exist
            if (!a.contacts.Contains(contactB))
            {
                // remove any contact that matches the name but not other parameters
                for (int i = a.contacts.Count - 1; i > -1; i--)
                {
                    if (a.contacts[i].cube.name.Equals(contactB.cube.name))
                    {
                        a.contacts.RemoveAt(i);
                    }
                }

                if (contactB.face == Vector3.down)
                {
                    a.gameObject.GetComponent<RigidBody3D>().Stop();
                    a.isGrounded = true;
                }
                //start here /////////////////////////////////////////////
                Vector3 minimunTranslationVectorAtoB = Vector3.zero;
                Vector3 collisionNormalAtoB = Vector3.zero;
                //check if it's a dynamic cube of static
                if (a.gameObject.GetComponent<RigidBody3D>().bodyType == BodyType.STATIC && b.gameObject.GetComponent<RigidBody3D>().bodyType == BodyType.DYNAMIC)
                {
                    movementScalarA = 0.0f;
                    movementScalarB = 1.0f;
                }
                if (a.gameObject.GetComponent<RigidBody3D>().bodyType == BodyType.DYNAMIC && b.gameObject.GetComponent<RigidBody3D>().bodyType == BodyType.STATIC)
                {
                    movementScalarA = 1.0f;
                    movementScalarB = 0.0f;
                }
                if (a.gameObject.GetComponent<RigidBody3D>().bodyType == BodyType.DYNAMIC && b.gameObject.GetComponent<RigidBody3D>().bodyType == BodyType.DYNAMIC)
                {
                    movementScalarA = 0.5f;
                    movementScalarB = 0.5f;
                }
                //check on which side it collides
                if (contactB.face == Vector3.up)
                {
                    collisionNormalAtoB = new Vector3(0, 1, 0);
                    minimunTranslationVectorAtoB = collisionNormalAtoB * penetration;
                }
                if (contactB.face == Vector3.forward)
                {
                    collisionNormalAtoB = new Vector3(0, 0, 1);
                    minimunTranslationVectorAtoB = collisionNormalAtoB * penetration;
                }
                if (contactB.face == Vector3.back)
                {  
                    collisionNormalAtoB = new Vector3(0, 0, -1);
                    minimunTranslationVectorAtoB = collisionNormalAtoB * penetration;
                }
                if (contactB.face == Vector3.left)
                {
                    collisionNormalAtoB = new Vector3(-1, 0, 0);
                    minimunTranslationVectorAtoB = collisionNormalAtoB * penetration;
                }
                if (contactB.face == Vector3.right)
                {
                    collisionNormalAtoB = new Vector3(1, 0, 0);
                    minimunTranslationVectorAtoB = collisionNormalAtoB * penetration;
                }

                a.transform.position += -minimunTranslationVectorAtoB * movementScalarA;
                b.transform.position += minimunTranslationVectorAtoB * movementScalarB;

                // add the new contact
                a.contacts.Add(contactB);
                a.isColliding = true;
            }
        }
        else
        {

            if (a.contacts.Exists(x => x.cube.gameObject.name == b.gameObject.name))
            {
                a.contacts.Remove(a.contacts.Find(x => x.cube.gameObject.name.Equals(b.gameObject.name)));
                a.isColliding = false;

                if (a.gameObject.GetComponent<RigidBody3D>().bodyType == BodyType.DYNAMIC)
                {
                    a.gameObject.GetComponent<RigidBody3D>().isFalling = true;
                    a.isGrounded = false;
                }
            }
        }
    }
}
