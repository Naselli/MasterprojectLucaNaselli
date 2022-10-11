using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
 
[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class ComputeShaderTest : MonoBehaviour {
    //Public fields
    public ComputeShader computeShader;
    public Transform paintSphere;
    public float radius;
 
    //Mesh related properties
    private Mesh mesh;
    private Material material;
    private int vertexCount;
 
    //Compute shader related properties
    private int kernelID;
    private int threadGroups;
    private ComputeBuffer vertexBuffer;
    private ComputeBuffer colorBuffer;
 
    private void OnEnable() {
        mesh = GetComponent<MeshFilter>().sharedMesh;
        material = GetComponent<MeshRenderer>().sharedMaterial;
        vertexCount = mesh.vertexCount;
 
        SetupBuffers();
        SetupData();
    }
 
    private void OnDisable() {
        DiscardBuffers();
    }


    private void SetupBuffers() { //Creates compute buffer
        vertexBuffer = new ComputeBuffer(vertexCount, sizeof(float) * 3, ComputeBufferType.Default, ComputeBufferMode.Immutable);
        colorBuffer = new ComputeBuffer(vertexCount, sizeof(float) * 4);
    }

    private void DiscardBuffers() { //Clear buffer to avoid memory leaks
        if (vertexBuffer != null) {
            vertexBuffer.Dispose();
            vertexBuffer = null;
        }
 
        if (colorBuffer != null) {
            colorBuffer.Dispose();
            colorBuffer = null;
        }
    }
 
    private void SetupData() {
        kernelID = computeShader.FindKernel("CSMain"); 
        computeShader.GetKernelThreadGroupSizes(kernelID, out uint threadGroupSizeX, out _, out _); 
        threadGroups = Mathf.CeilToInt((float)vertexCount / threadGroupSizeX);
 
        using (var meshDataArray = Mesh.AcquireReadOnlyMeshData(mesh)) {
            var meshData = meshDataArray[0];
            using (var vertexArray = new NativeArray<Vector3>(vertexCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory)) {
                meshData.GetVertices(vertexArray);
                vertexBuffer.SetData(vertexArray);
            }
        }
 
        //Static data
        computeShader.SetBuffer(kernelID, "_VertexBuffer", vertexBuffer);
        computeShader.SetBuffer(kernelID, "_ColorBuffer", colorBuffer);
        computeShader.SetInt("_VertexCount", vertexCount);
 
        material.SetBuffer("_ColorBuffer", colorBuffer);
    }
 
    private void Update() {
        //Dynamic data
        computeShader.SetMatrix("_LocalToWorld", transform.localToWorldMatrix);
        computeShader.SetVector("_Sphere", new Vector4(paintSphere.position.x, paintSphere.position.y, paintSphere.position.z, radius));
 
        computeShader.Dispatch(kernelID, threadGroups, 1, 1);
 
    }
 
    private void OnDrawGizmos() {
        if (paintSphere != null) {
            Gizmos.DrawWireSphere(paintSphere.position, radius);
        }
    }
 
}