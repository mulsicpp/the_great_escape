from enum import IntEnum
import os
import struct
from typing import List, Tuple
class TelemetryEvent(IntEnum):
    GAME_WON = 0
    LEFT = 1
    RIGHT = 2
    UP = 3
    DOWN = 4
    FORWARD = 5
    BUMP = 6
    STICKER = 7


TelemetryEntry = Tuple[TelemetryEvent, int]


class TelemetryData:
    def __init__(self, seed: int, maze_size: int, events: List[TelemetryEntry]):
        self.seed = seed
        self.maze_size = maze_size
        self.events = events


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
                print(f"  Seed: {telemetry.seed}")
                print(f"  Maze Size: {telemetry.maze_size}")
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
                print(f"  Seed: {telemetry.seed}")
                print(f"  Maze Size: {telemetry.maze_size}")
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
        winners.sort(key=lambda x: x.maze_size)

        for w in winners:
            print(f"  Seed: {w.seed}")
            print(f"  Maze Size: {w.maze_size}")
            print(f"  Won in {w.events[-1][1]}ms and {len(w.events)} Actions")
            print()



    losers_and_amount_of_actions_sorted_by_size = 1
    if losers_and_amount_of_actions_sorted_by_size:
        print()
        print("Losers and how many actions they did, sorted by maze size:")
        print()
        winners = []
        for filename, telemetry in iter_telemetry_files(current_dir):
            if len(telemetry.events) > 0 and telemetry.events[len(telemetry.events) - 1][0] != TelemetryEvent.GAME_WON:
                winners.append(telemetry)
        winners.sort(key=lambda x: x.maze_size)

        for w in winners:
            print(f"  Seed: {w.seed}")
            print(f"  Maze Size: {w.maze_size}")
            print(f"  Gave up after {w.events[-1][1]}ms and {len(w.events)} Actions")
            print()

