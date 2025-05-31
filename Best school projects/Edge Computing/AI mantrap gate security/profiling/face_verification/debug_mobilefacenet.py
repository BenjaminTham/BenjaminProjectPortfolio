import time
import cv2
import numpy as np
import tensorflow as tf
import mediapipe as mp
import psutil
import os
import cProfile
import pstats
import tracemalloc
import sklearn.preprocessing

def get_graph_tensor_names(graph):
    """Get all tensor names from the graph"""
    return [op.name for op in graph.get_operations()]

def print_model_info(model_type, model):
    """Print detailed information about model architecture and memory usage"""
    print(f"\n{'-'*20} {model_type} Info {'-'*20}")
    
    if isinstance(model, tf.lite.Interpreter):
        # TFLite model info
        input_details = model.get_input_details()
        output_details = model.get_output_details()
        
        print(f"Input Details:")
        print(f"Shape: {input_details[0]['shape']}")
        print(f"Type: {input_details[0]['dtype']}")
        
        print(f"\nOutput Details:")
        print(f"Shape: {output_details[0]['shape']}")
        print(f"Type: {output_details[0]['dtype']}")
        
        # Get memory usage
        memory_mb = model.get_tensor(input_details[0]['index']).nbytes / 1024 / 1024
        print(f"\nTensor Memory: {memory_mb:.2f} MB")
        
    elif isinstance(model, tf.Graph):
        # Frozen model info
        print("Available tensor names in graph:")
        tensor_names = get_graph_tensor_names(model)
        # for name in tensor_names:
        #     print(f"  {name}")
        
        # Find input and output tensors (assuming they contain 'input' and 'output' or 'embedding' in their names)
        input_tensor_name = None
        output_tensor_name = None
        for name in tensor_names:
            if 'input' in name.lower():
                input_tensor_name = f"{name}:0"
            elif 'output' in name.lower() or 'embedding' in name.lower():
                output_tensor_name = f"{name}:0"
        
        if not input_tensor_name or not output_tensor_name:
            print("Could not automatically detect input/output tensor names")
            return
            
        with model.as_default():
            input_tensor = model.get_tensor_by_name(input_tensor_name)
            output_tensor = model.get_tensor_by_name(output_tensor_name)
            
            print(f"\nInput Details:")
            print(f"Name: {input_tensor_name}")
            print(f"Shape: {input_tensor.shape}")
            print(f"Type: {input_tensor.dtype}")
            
            print(f"\nOutput Details:")
            print(f"Name: {output_tensor_name}")
            print(f"Shape: {output_tensor.shape}")
            print(f"Type: {output_tensor.dtype}")
    else:
        print(f"Model type is: {type(model)}")

def l2_normalize(x):
    """L2 normalize the input array"""
    return x / np.sqrt(np.maximum(np.sum(np.square(x), axis=1, keepdims=True), 1e-12))

def test_model_inference(model_type, model, test_input):
    """Test model inference and measure performance"""
    print(f"\n{'-'*20} {model_type} Inference Test {'-'*20}")
    
    start_time = time.time()
    process = psutil.Process()
    initial_memory = process.memory_info().rss / 1024 / 1024
    
    if isinstance(model, tf.lite.Interpreter):
        input_details = model.get_input_details()
        output_details = model.get_output_details()
        
        model.set_tensor(input_details[0]['index'], test_input)
        model.invoke()
        output = model.get_tensor(output_details[0]['index'])
        
    elif isinstance(model, tf.Graph):
        tensor_names = get_graph_tensor_names(model)
        input_tensor_name = next((f"{name}:0" for name in tensor_names if 'input' in name.lower()), None)
        output_tensor_name = next((f"{name}:0" for name in tensor_names if 'output' in name.lower() or 'embedding' in name.lower()), None)
        
        if not input_tensor_name or not output_tensor_name:
            print("Could not find input/output tensor names")
            return None
            
        with tf.compat.v1.Session(graph=model) as sess:
            input_tensor = model.get_tensor_by_name(input_tensor_name)
            output_tensor = model.get_tensor_by_name(output_tensor_name)
            output = sess.run(output_tensor, feed_dict={input_tensor: test_input})

    inference_time = time.time() - start_time
    memory_used = process.memory_info().rss / 1024 / 1024 - initial_memory
    
    print(f"Inference Time: {inference_time:.4f} seconds")
    print(f"Memory Used: {memory_used:.2f} MB")
    print(f"Output Shape: {output.shape}")
    print(f"Raw Output Stats:")
    print(f"- Mean: {np.mean(output):.4f}")
    print(f"- Std: {np.std(output):.4f}")
    print(f"- Min: {np.min(output):.4f}")
    print(f"- Max: {np.max(output):.4f}")
    
    # Add L2 normalization
    normalized_output = l2_normalize(output)
    print(f"\nNormalized Output Stats:")
    print(f"- Mean: {np.mean(normalized_output):.4f}")
    print(f"- Std: {np.std(normalized_output):.4f}")
    print(f"- Min: {np.min(normalized_output):.4f}")
    print(f"- Max: {np.max(normalized_output):.4f}")
    
    return normalized_output  # Return normalized output instead

def main():
    # Load TFLite model
    print("Loading MobileFaceNet TFLite model...")
    interpreter = tf.lite.Interpreter(model_path="mobilefacenet.tflite")
    interpreter.allocate_tensors()

    # Load frozen model
    print("\nLoading MobileFaceNet frozen model...")
    with open("mobilefacenet_tf.pb", "rb") as f:
        graph_def = tf.compat.v1.GraphDef()
        graph_def.ParseFromString(f.read())
    
    frozen_graph = tf.Graph()
    with frozen_graph.as_default():
        tf.compat.v1.import_graph_def(graph_def, name="")

    # Create test input
    test_input = np.random.random((1, 112, 112, 3)).astype(np.float32)
    test_input = (test_input - 127.5) / 128.0  # Normalize like in your preprocessing
    
    # Print model information
    print_model_info("TFLite Model", interpreter)
    print_model_info("Frozen Model", frozen_graph)
    
    # Test inference
    tflite_output = test_model_inference("TFLite Model", interpreter, test_input)
    frozen_output = test_model_inference("Frozen Model", frozen_graph, test_input)
    
    # Compare outputs
    print("\n--- Output Comparison ---")
    if tflite_output is not None and frozen_output is not None:
        # Both outputs are now L2 normalized
        cosine_similarity = 1 - np.dot(frozen_output.flatten(), tflite_output.flatten())
        print(f"Cosine Similarity between Frozen and TFLite outputs (L2 normalized): {cosine_similarity:.4f}")
        print(f"Mean Absolute Difference (L2 normalized): {np.mean(np.abs(frozen_output - tflite_output)):.4f}")
    
if __name__ == "__main__":
    main()