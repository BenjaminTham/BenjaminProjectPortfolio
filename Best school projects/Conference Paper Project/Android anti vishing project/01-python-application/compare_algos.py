import importlib.util
import os
import time
import pandas as pd

def load_detection_modules(path='detection_modules'):
    modules = {}
    for filename in os.listdir(path):
        if filename.endswith('.py'):
            spec = importlib.util.spec_from_file_location(filename[:-3], os.path.join(path, filename))
            module = importlib.util.module_from_spec(spec)
            spec.loader.exec_module(module)
            modules[filename[:-3]] = module
    return modules

def detect_vishing(input_text, modules):
    results = {}
    preprocess_times = {}
    for module_name, module in modules.items():
        start = time.time()
        result = module.detect_vishing(input_text)
        end = time.time()
        results[module_name] = {'result': result, 'time': end - start}
        preprocess_times[module_name] = module.PREPROCESSING_TIME
    return results, preprocess_times

if __name__ == "__main__":
    # Load input data
    input_data = pd.read_csv('resources/test_cases.csv')

    # Load detection modules
    modules = load_detection_modules()

    processing_times = {module_name: [] for module_name in modules}
    preprocess_times = {module_name: [] for module_name in modules}
    correct_predictions = {module_name: 0 for module_name in modules}
    total_predictions = {module_name: 0 for module_name in modules}

    for index, row in input_data.iterrows():
        transcript = row['Transcript']
        actual_label = row['Label']
        print(f'======================================================')
        print(f'\nTest Case Index: {index}')
        print(f'Transcript: {transcript}')
        print(f'Actual Label: {actual_label}\n')

        results, preprocess_times_case = detect_vishing(transcript, modules)

        for module_name, result_info in results.items():
            predicted_label = result_info["result"][0]
            print(f'Module: {module_name}')
            print(f'Predicted label: {predicted_label}')
            print(f'Full Results: {result_info["result"]}')
            print(f'Time taken: {result_info["time"]}s\n')
            processing_times[module_name].append(result_info['time'])

            # Update correct predictions and total predictions
            total_predictions[module_name] += 1
            if predicted_label == actual_label:
                correct_predictions[module_name] += 1

        for module_name, preprocess_time in preprocess_times_case.items():
            preprocess_times[module_name].append(preprocess_time)

    print(f'=========== CONCLUSION ===========')

    for module_name in modules:
        print(f'\nModule: {module_name}')
        avg_processing_time = sum(processing_times[module_name])/len(processing_times[module_name])
        avg_preprocessing_time = sum(preprocess_times[module_name])/len(preprocess_times[module_name])
        accuracy = correct_predictions[module_name] / total_predictions[module_name]
        print(f'Accuracy: {(accuracy*100):.1f}%')
        print(f'Average processing time: {avg_processing_time}s')
        print(f'Average preprocessing time: {avg_preprocessing_time:}s')
print()