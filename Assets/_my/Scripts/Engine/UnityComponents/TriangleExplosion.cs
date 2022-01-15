using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

// Usage: StartCoroutine(gameObject.GetComponent<TriangleExplosion().SplitMesh(data));

namespace Smr.Components {
    [Serializable]
    public class TriangleExplosionData {
        public bool NeedToDestroyOrigin = true;
        public Vector2 XRange;
        public Vector2 YRange;
        public Vector2 ZRange;
        public Vector2 ExplosionForceRange;
        public float ExplosionRadius;
        public Vector2 LifetimeRange;
        public float UpdateTimeDelta;
    }

    public class TriangleExplosion : MonoBehaviour {
        public IEnumerator SplitMesh(TriangleExplosionData data, Action<Rigidbody> RigidBodyCreationClosure = null) {
            if (GetComponent<MeshFilter>() == null || GetComponent<SkinnedMeshRenderer>() == null) {
                yield return null;
            }

            if (GetComponent<Collider>()) {
                GetComponent<Collider>().enabled = false;
            }

            var mesh = new Mesh();
            if (GetComponent<MeshFilter>()) {
                mesh = GetComponent<MeshFilter>().mesh;
            } else if (GetComponent<SkinnedMeshRenderer>()) {
                mesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
            }

            var materials = new Material[0];
            if (GetComponent<MeshRenderer>()) {
                materials = GetComponent<MeshRenderer>().materials;
            } else if (GetComponent<SkinnedMeshRenderer>()) {
                materials = GetComponent<SkinnedMeshRenderer>().materials;
            }

            var gameObjectCache = gameObject;
            var transformCache = transform;
            var currentPosition = transformCache.position;

            var verts = mesh.vertices;
            var normals = mesh.normals;
            var uvs = mesh.uv;
            for (int subMeshIndex = 0; subMeshIndex < mesh.subMeshCount; subMeshIndex++) {
                var indices = mesh.GetTriangles(subMeshIndex);

                for (int i = 0; i < indices.Length; i += 3) {
                    var newVerts = new Vector3[3];
                    var newNormals = new Vector3[3];
                    var newUvs = new Vector2[3];
                    for (int n = 0; n < 3; n++) {
                        var index = indices[i + n];
                        newVerts[n] = verts[index];
                        newUvs[n] = uvs[index];
                        newNormals[n] = normals[index];
                    }

                    var triangle = new GameObject($"{gameObjectCache.name}_Part_{i / 3}") {
                        layer = gameObjectCache.layer
                    };

                    triangle.transform.position = currentPosition;
                    triangle.transform.rotation = transformCache.rotation;
                    triangle.AddComponent<MeshRenderer>().material = materials[subMeshIndex];
                    triangle.AddComponent<MeshFilter>().mesh = new Mesh {
                        vertices = newVerts,
                        normals = newNormals,
                        uv = newUvs,
                        triangles = new[] { 0, 1, 2, 2, 1, 0 }
                    };
                    triangle.AddComponent<BoxCollider>();

                    var explosionPower = Random.Range(data.ExplosionForceRange.x, data.ExplosionForceRange.y);
                    var explosionPos = new Vector3(
                        currentPosition.x + Random.Range(data.XRange.x, data.XRange.y),
                        currentPosition.y + Random.Range(data.YRange.x, data.YRange.y),
                        currentPosition.z + Random.Range(data.ZRange.x, data.ZRange.y));

                    var triangleRigidbody = triangle.AddComponent<Rigidbody>();
                    RigidBodyCreationClosure?.Invoke(triangleRigidbody);
                    triangleRigidbody.AddExplosionForce(explosionPower, explosionPos, data.ExplosionRadius);

                    Destroy(triangle, Random.Range(data.LifetimeRange.x, data.LifetimeRange.y));
                }
            }

            GetComponent<Renderer>().enabled = false;

            yield return new WaitForSeconds(data.UpdateTimeDelta);
            if (data.NeedToDestroyOrigin) {
                Destroy(gameObject);
            }
        }
    }
}