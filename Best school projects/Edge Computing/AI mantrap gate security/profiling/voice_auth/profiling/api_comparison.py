from time import time
import cProfile
import pstats
import io
import psutil
import os
import speech_recognition as sr
from contextlib import contextmanager
from typing import Dict, Any

class Profiler:
    def __init__(self):
        self.process = psutil.Process()
        self.pr = cProfile.Profile()
        
    @contextmanager
    def profile(self):
        # Get initial metrics
        start_time = time()
        start_mem = self.process.memory_info().rss / 1024 / 1024  # MB
        
        # Get initial I/O metrics
        try:
            start_io = self.process.io_counters()
            start_net = psutil.net_io_counters()
            self.io_available = True
        except (psutil.Error, AttributeError):
            self.io_available = False
            
        self.pr.enable()
        
        yield
        
        # Get final metrics
        self.pr.disable()
        end_time = time()
        end_mem = self.process.memory_info().rss / 1024 / 1024  # MB
        
        # Calculate differences
        self.duration = round(end_time - start_time, 2)
        self.memory_change = round(end_mem - start_mem, 2)
        
        if self.io_available:
            end_io = self.process.io_counters()
            end_net = psutil.net_io_counters()
            
            # Disk I/O in KB
            self.io_read = round((end_io.read_bytes - start_io.read_bytes) / 1024, 2)
            self.io_write = round((end_io.write_bytes - start_io.write_bytes) / 1024, 2)
            
            # Network I/O in KB
            self.net_sent = round((end_net.bytes_sent - start_net.bytes_sent) / 1024, 2)
            self.net_recv = round((end_net.bytes_recv - start_net.bytes_recv) / 1024, 2)
        else:
            self.io_read = self.io_write = self.net_sent = self.net_recv = 0
        
        # Get function calls
        s = io.StringIO()
        ps = pstats.Stats(self.pr, stream=s).sort_stats('cumulative')
        ps.print_stats()
        self.function_stats = s.getvalue()
    
    def get_total_calls(self):
        """Returns tuple of (total_calls, primitive_calls)"""
        stats_lines = self.function_stats.split('\n')
        if len(stats_lines) > 0:
            first_line = stats_lines[0].strip()
            if 'function calls' in first_line:
                parts = first_line.split('function calls')[0].strip()
                if '(' in first_line:
                    total_calls = int(parts)
                    primitive_calls = int(first_line.split('(')[1].split()[0])
                    return (total_calls, primitive_calls)
                else:
                    calls = int(parts)
                    return (calls, calls)
        return (0, 0)

# load audio file
r = sr.Recognizer()
with sr.AudioFile('for_profiling.wav') as source:
    print("Loading audio file...")
    audio = r.record(source)

# Keys
witai_key = ""
houndify_client_id = ""
houndify_client_key = ""

apis = [
    {
        "name": "Sphnix",
        "recognise": lambda a: r.recognize_sphinx(a)
    },
    {
        "name": "Google",
        "recognise": lambda a: r.recognize_google(a)
    },
    {
        "name": "Wit.ai",
        "recognise": lambda a: r.recognize_wit(a, key=witai_key)
    },
    {
        "name": "Houndify",
        "recognise": lambda a: r.recognize_houndify(a, client_id=houndify_client_id, client_key=houndify_client_key)[0]
    },
    {
        "name": "Whisper",
        "recognise": lambda a: r.recognize_whisper(a, language="english")
    }
]

results = {}

for api in apis:
    name, recognise = api["name"], api["recognise"]
    profiler = Profiler()
    
    try:
        with profiler.profile():
            result = recognise(audio)
        
        print(f"\n=== {name} Results ===")
        print(f"Text: {result}")
        print(f"Time taken: {profiler.duration} seconds")
        print(f"Memory change: {profiler.memory_change} MB")
        print(f"Disk read: {profiler.io_read} KB")
        print(f"Disk write: {profiler.io_write} KB")
        print(f"Network sent: {profiler.net_sent} KB")
        print(f"Network received: {profiler.net_recv} KB")
        print("\nFunction Call Statistics:")
        print(profiler.function_stats[:500])  # Print first 500 chars of function stats
        
        results[name] = {
            'time': profiler.duration,
            'memory': profiler.memory_change,
            'io_read': profiler.io_read,
            'io_write': profiler.io_write,
            'net_sent': profiler.net_sent,
            'net_recv': profiler.net_recv,
            'calls': profiler.get_total_calls()[0],  # Use total calls
            'primitive_calls': profiler.get_total_calls()[1],  # Add primitive calls
            'text': result  # Store the recognized text
        }
        
    except sr.UnknownValueError:
        print(f"{name} could not understand audio")
    except sr.RequestError as e:
        print(f"Could not request results from {name} {e}")

print("\nSummary:")
headers = ['API', 'Time(s)', 'Memory(MB)', 'Net Send(KB)', 'Net Recv(KB)', 'Disk R/W(KB)', 'Calls(T/P)']
print(" | ".join(h.center(12) for h in headers))
print("-" * 97)
for api_name, metrics in results.items():
    values = [
        api_name,
        str(metrics['time']),
        str(metrics['memory']),
        str(metrics['net_sent']),
        str(metrics['net_recv']),
        f"{metrics['io_read']}/{metrics['io_write']}",
    ]
    print(" | ".join(v.center(12) for v in values))

# Add text output comparison
print("\nText Output Comparison:")
print("-" * 80)
for api_name, metrics in results.items():
    print(f"{api_name.ljust(10)}: {metrics.get('text', 'No output')}")
