from collections import defaultdict
from enum import IntEnum
import os
import struct
from typing import List, Tuple

import matplotlib.pyplot as plt
from matplotlib.ticker import PercentFormatter

class TelemetryEvent(IntEnum):
    GAME_WON = 0
    LEFT = 1
    RIGHT = 2
    UP = 3
    DOWN = 4
    FORWARD = 5
    BUMP = 6
    STICKER = 7

class AbstractTelemetryEvent(IntEnum):
    TURN = 0
    STEP = 1
    BUMP = 2
    STICKER = 3

class Direction(IntEnum):
    LEFT = 0
    RIGHT = 1
    UP = 2
    DOWN = 3


TelemetryEntry = Tuple[TelemetryEvent, int]


class TelemetryData:
    def __init__(self, seed: int, maze_size: int, events: List[TelemetryEntry]):
        self._seed = seed
        self._maze_size = maze_size
        self.events = events

    def completed(self):
        if len(self.events) > 0 and self.events[-1][0] == TelemetryEvent.GAME_WON:
            return True
        else:
            return False
        
    def duration(self):
        return self.events[-1][1] if len(self.events) > 0 else 0.0
    
    def action_count(self):
        return len(self.events)
    
    def step_count(self):
        return len([val for val, t in self.events if val == TelemetryEvent.FORWARD])
    
    def sticker_count(self):
        return len([val for val, t in self.events if val == TelemetryEvent.STICKER]) 
        


def format_duration(millis: int) -> str:
    seconds = millis / 1000
    minutes, seconds = divmod(seconds, 60)
    hours, minutes = divmod(minutes, 60)

    parts = []
    if hours:
        parts.append(f"{hours} h")
    if minutes:
        parts.append(f"{minutes} min")
    parts.append(f"{seconds} s")

    return " ".join(parts)

def read_telemetry_file(path: str) -> TelemetryData:
    events: List[TelemetryEntry] = []

    with open(path, "rb") as f:

        seed_bytes = f.read(4)
        size_bytes = f.read(4)

        if len(seed_bytes) < 4 or len(size_bytes) < 4:
            raise ValueError("Datei zu kurz (Header fehlt)")

        seed = struct.unpack("<I", seed_bytes)[0]
        maze_size = struct.unpack("<I", size_bytes)[0]


        while True:
            chunk = f.read(8)
            if len(chunk) < 8:
                break

            event_id, timestamp = struct.unpack("<II", chunk)

            try:
                event = TelemetryEvent(event_id)
            except ValueError:
                raise ValueError(f"Unbekanntes TelemetryEvent: {event_id}")

            events.append((event, timestamp))

    return TelemetryData(seed, maze_size, events)

def iter_telemetry_files(directory: str):
    for filename in os.listdir(directory):

        if filename == "Reader.py":
            continue

        if filename.endswith(".png"):
            continue

        path = os.path.join(directory, filename)


        if not os.path.isfile(path):
            continue

        try:
            telemetry = read_telemetry_file(path)
            yield filename, telemetry
        except Exception as e:
            print(f"Überspringe {filename}: {e}")

def markers_used(events):
    m = 0
    for event, timestamp in events:
        if event == 7:
            m += 1
    return m

def group_by(pairs, grouping_function):
    group_values = defaultdict(list)

    for key, value in pairs:
        group_values[key].append(value)
    
    group_pairs = list()

    for key, values in group_values.items():
        group_pairs.append((key, grouping_function(values)))
    return sorted(group_pairs)


def completed_with_size(telemetry: TelemetryData):
    if len(telemetry.events) > 0 and telemetry.events[-1][0] == TelemetryEvent.GAME_WON:
        return (telemetry._maze_size, True)
    else:
        return (telemetry._maze_size, False)
    


# Grouping functions
def true_rate(values: list):
    return len([val for val in values if val == True]) / len(values)

def sum(values: list):
    sum_value = 0

    for val in values:
        sum_value += val
    return sum_value

def average(values: list):
     return sum(values) / len(values)


    

if __name__ == "__main__":
    current_dir = os.path.dirname(os.path.abspath(__file__))

    # Setzen zum ausgeben
    winners_and_markers = 0
    if winners_and_markers:
        print()
        print("Winners and how many Markers they used:")
        print()
        for filename, telemetry in iter_telemetry_files(current_dir):
            if len(telemetry.events) > 0 and telemetry.events[-1][0] == TelemetryEvent.GAME_WON:
                print(f"  Seed: {telemetry._seed}")
                print(f"  Maze Size: {telemetry._maze_size}")
                print(f"  Won in {telemetry.events[-1][1]}ms")
                print(f"  Used {markers_used(telemetry.events)} Markers")
                print()



    losers_and_markers = 0
    if losers_and_markers:
        print()
        print("Losers and how many Markers they used:")
        print()
        for filename, telemetry in iter_telemetry_files(current_dir):
            if len(telemetry.events) > 0 and telemetry.events[-1][0] != TelemetryEvent.GAME_WON:
                print(f"  Seed: {telemetry._seed}")
                print(f"  Maze Size: {telemetry._maze_size}")
                print(f"  Gave up after {telemetry.events[-1][1]}ms")
                print(f"  Used {markers_used(telemetry.events)} Markers")
                print()



    winners_and_amount_of_actions_sorted_by_size = 0
    if winners_and_amount_of_actions_sorted_by_size:
        print()
        print("Winners and how many actions they did, sorted by maze size:")
        print()
        winners = []
        for filename, telemetry in iter_telemetry_files(current_dir):
            if len(telemetry.events) > 0 and telemetry.events[len(telemetry.events) - 1][0] == TelemetryEvent.GAME_WON:
                winners.append(telemetry)
        winners.sort(key=lambda x: x._maze_size)

        for w in winners:
            print(f"  Seed: {w._seed}")
            print(f"  Maze Size: {w._maze_size}")
            print(f"  Won in {w.events[-1][1]}ms and {len(w.events)} Actions")
            print()



    losers_and_amount_of_actions_sorted_by_size = 0
    if losers_and_amount_of_actions_sorted_by_size:
        print()
        print("Losers and how many actions they did, sorted by maze size:")
        print()
        winners = []
        for filename, telemetry in iter_telemetry_files(current_dir):
            if len(telemetry.events) > 0 and telemetry.events[len(telemetry.events) - 1][0] != TelemetryEvent.GAME_WON:
                winners.append(telemetry)
        winners.sort(key=lambda x: x._maze_size)

        for w in winners:
            print(f"  Seed: {w._seed}")
            print(f"  Maze Size: {w._maze_size}")
            print(f"  Gave up after {w.events[-1][1]}ms and {len(w.events)} Actions")
            print()

    telemetries = [t for _, t in iter_telemetry_files(current_dir)]

    avg_game_duration = average([t.duration() for t in telemetries])

    print(f"Average game duration: {format_duration(avg_game_duration)}")

    os.makedirs(f"{current_dir}/diag", exist_ok=True)

    data = group_by([(t._maze_size, t.completed()) for t in telemetries], true_rate)
    for size, won in data:
        print(f"Size: {size} Won: {won}")
    
    if data:
        x, y = zip(*data)
    else:
        x, y = [], []

    plt.figure(figsize=(6, 4), dpi=200)
    plt.bar(x, y)
    plt.xlabel("Maze Size")
    plt.xticks(x)

    plt.ylabel("Completion Rate")
    plt.gca().yaxis.set_major_formatter(PercentFormatter(1.0))

    plt.savefig(f"{current_dir}/diag/completion_rate_by_size.png")
    plt.close()

    data = group_by([(t._maze_size, t.duration() / 60000) for t in telemetries], average)
    for size, duration in data:
        print(f"Size: {size} Duration: {duration}")
    
    if data:
        x, y = zip(*data)
    else:
        x, y = [], []

    plt.figure(figsize=(6, 4), dpi=200)
    plt.bar(x, y)
    plt.xlabel("Maze Size")
    plt.xticks(x)

    plt.ylabel("Avg. Game Duration (min)")

    plt.savefig(f"{current_dir}/diag/game_duration_by_size.png")
    plt.close()

    data = group_by([(t._maze_size, t.step_count()) for t in telemetries if t.completed()], average)
    for size, steps in data:
        print(f"Size: {size} Steps: {steps}")
    
    if data:
        x, y = zip(*data)
    else:
        x, y = [], []

    plt.figure(figsize=(6, 4), dpi=200)
    plt.bar(x, y)
    plt.xlabel("Maze Size")
    plt.xticks(x)

    plt.ylabel("Avg. Step Count")

    plt.savefig(f"{current_dir}/diag/steps_by_size.png")
    plt.close()

    abstract_event_map = defaultdict(AbstractTelemetryEvent)

    abstract_event_map[TelemetryEvent.LEFT] = AbstractTelemetryEvent.TURN
    abstract_event_map[TelemetryEvent.RIGHT] = AbstractTelemetryEvent.TURN
    abstract_event_map[TelemetryEvent.UP] = AbstractTelemetryEvent.TURN
    abstract_event_map[TelemetryEvent.DOWN] = AbstractTelemetryEvent.TURN

    abstract_event_map[TelemetryEvent.FORWARD] = AbstractTelemetryEvent.STEP
    abstract_event_map[TelemetryEvent.BUMP] = AbstractTelemetryEvent.BUMP
    abstract_event_map[TelemetryEvent.STICKER] = AbstractTelemetryEvent.STICKER

    events_completed = list()
    events_uncompleted = list()

    for t in telemetries:
        if t.completed():
            events_completed += t.events
        else:
            events_uncompleted += t.events

    data = group_by([(abstract_event_map.get(e), 0) for e, _ in events_completed + events_uncompleted if e != TelemetryEvent.GAME_WON], len)

    if data:
        labels, vals = zip(*data)
    else:
        labels, vals = [], []

    plt.figure(figsize=(6, 4), dpi=200)
    plt.pie(vals, autopct="%1.1f%%", pctdistance=1.15)

    plt.legend(["Turn", "Step", "Bump", "Marker"], title="Actions")
    plt.axis("equal")
    
    plt.savefig(f"{current_dir}/diag/action_distribution.png")
    plt.close()

    data = group_by([(abstract_event_map.get(e), 0) for e, _ in events_completed if e != TelemetryEvent.GAME_WON], len)

    if data:
        labels, vals = zip(*data)
    else:
        labels, vals = [], []

    plt.figure(figsize=(6, 4), dpi=200)
    plt.pie(vals, autopct="%1.1f%%", pctdistance=1.15)

    plt.legend(["Turn", "Step", "Bump", "Marker"], title="Actions")
    plt.axis("equal")
    
    plt.savefig(f"{current_dir}/diag/action_distribution_winners.png")
    plt.close()

    data = group_by([(abstract_event_map.get(e), 0) for e, _ in events_uncompleted if e != TelemetryEvent.GAME_WON], len)

    if data:
        labels, vals = zip(*data)
    else:
        labels, vals = [], []

    plt.figure(figsize=(6, 4), dpi=200)
    plt.pie(vals, autopct="%1.1f%%", pctdistance=1.15)

    plt.legend(["Turn", "Step", "Bump", "Marker"], title="Actions")
    plt.axis("equal")
    
    plt.savefig(f"{current_dir}/diag/action_distribution_losers.png")
    plt.close()

    all_events = events_completed + events_uncompleted

    miss_input_count = [0] * 4
    total_input_count = [0] * 4

    for i in range(0, len(all_events) - 2):
        e = int(all_events[i][0])
        if e in range(1, 5):
            dir = e - 1
            total_input_count[dir] += 1
            inv_e = (dir ^ 1) + 1

            if int(all_events[i + 1][0]) == inv_e and int(all_events[i + 2][0]) == inv_e:
                miss_input_count[dir] += 1
    
    print(miss_input_count)

    for i in range(4):
        miss_input_count[i] /= total_input_count[i]

    plt.figure(figsize=(6, 4), dpi=200)
    plt.bar(["Left", "Right", "Up", "Down"], miss_input_count, color=["green", "green", "red", "red"])

    plt.xlabel("Direction")

    plt.ylabel("Correction Rate")
    plt.gca().yaxis.set_major_formatter(PercentFormatter(1.0))
    
    plt.savefig(f"{current_dir}/diag/miss_inputs.png")
    plt.close()


