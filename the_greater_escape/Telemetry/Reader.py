from enum import IntEnum

class TelemetryEvent(IntEnum):
    GAME_WON = 0
    LEFT = 1
    RIGHT = 2
    UP = 3
    DOWN = 4
    FORWARD = 5
    BUMP = 6
    STICKER = 7



import struct
from typing import List, Tuple

TelemetryEntry = Tuple[TelemetryEvent, int]


class TelemetryData:
    def __init__(self, seed: int, maze_size: int, events: List[TelemetryEntry]):
        self.seed = seed
        self.maze_size = maze_size
        self.events = events


def read_telemetry_file(path: str) -> TelemetryData:
    events: List[TelemetryEntry] = []

    with open(path, "rb") as f:
        # Header lesen
        seed_bytes = f.read(4)
        size_bytes = f.read(4)

        if len(seed_bytes) < 4 or len(size_bytes) < 4:
            raise ValueError("Datei zu kurz (Header fehlt)")

        seed = struct.unpack("<I", seed_bytes)[0]
        maze_size = struct.unpack("<I", size_bytes)[0]

        # Events lesen
        while True:
            chunk = f.read(8)  # 4 Byte Event + 4 Byte Timestamp
            if len(chunk) < 8:
                break

            event_id, timestamp = struct.unpack("<II", chunk)

            try:
                event = TelemetryEvent(event_id)
            except ValueError:
                raise ValueError(f"Unbekanntes TelemetryEvent: {event_id}")

            events.append((event, timestamp))

    return TelemetryData(seed, maze_size, events)

telemetry = read_telemetry_file("Game-9862387036482753903")

print("Seed:", telemetry.seed)
print("Maze Size:", telemetry.maze_size)

# Array aus (Event, Timestamp)
actions = telemetry.events

# Beispiel: alle FORWARD-Aktionen
forward_events = [t for e, t in actions if e == TelemetryEvent.FORWARD]

# Beispiel: Events chronologisch durchgehen
for event, timestamp in actions:
    print(f"{timestamp:6d} ms -> {event.name}")
