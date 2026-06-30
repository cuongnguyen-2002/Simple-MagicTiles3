# JSON Level Design README

## 1\. Root structure

The file is an `MT3` level-design JSON with three top-level sections:

- `notes`: the timeline of level events and playable objects.  
- `songMeta`: timing and lane configuration for the chart.  
- `format`: the chart format identifier, set to `MT3`.

## 2\. `songMeta`

`songMeta` defines the basic chart setup:

- `bpm`: `130`  
- `nLanes`: `4`  
- `visualSpeed`: `4.333333333333333`  
- `audioDuration`: `89.78285` seconds

## 3\. Note object schema

Each item inside `notes` represents one level event.

### Base structure

```json
{
  "lane": 1,
  "time": 0.0,
  "type": "short",
  "metas": [],
  "controls": [],
  "duration": 0.0
}
```

### Field reference

#### Core fields

- `lane`: Lane index where the event is placed. This level uses 4 lanes.  
- `time`: Event start time in seconds from the beginning of the song.  
- `type`: Event type, such as `short`, `long`, `zigzag`, or `mood`.

#### Optional fields

- `metas`: Extra key/value metadata attached to the event.  
- `controls`: Control points used by notes that have movement or sustain behavior.  
- `duration`: Duration in seconds. Usually present on notes that span time, such as `long` and `zigzag`.

### Usage notes

- Simple tap notes usually have empty `metas`, empty `controls`, and no `duration`.  
- Sustained or path-based notes usually include both `controls` and `duration`.  
- Visual events such as `mood` mainly use `metas` to store presentation data like `bg_color`.

## 4\. Note types used in this level

### `short`

A basic instantaneous note. It has a single `lane`, a `time`, and usually an empty `controls` array.

Example:

```json
{
  "lane": 4,
  "time": 0.46153799999999995,
  "type": "short",
  "metas": [],
  "controls": []
}
```

### `long`

A sustained note. It includes `controls` and `duration`, which define how long the note lasts and where the sustain endpoint is placed.

Example:

```json
{
  "lane": 3,
  "time": 1.8461519999999998,
  "type": "long",
  "metas": [],
  "controls": [
    {
      "lane": 3,
      "time": 2.3076899999999996
    }
  ],
  "duration": 0.4615379999999998
}
```

### `zigzag (Optional)`

A directional or path-based note. In this file it also uses `controls` and `duration`, similar to `long`, but is explicitly marked with type `zigzag`.

Example:

```json
{
  "lane": 3,
  "time": 22.615361999999998,
  "type": "zigzag",
  "metas": [],
  "controls": [
    {
      "lane": 4,
      "time": 23.0769
    }
  ],
  "duration": 0.4615380000000009
}
```

### `mood`

A chart event used for visual or thematic changes. In this file, `mood` notes carry `bg_color` metadata such as `yellow`, `purple`, `blue`, and `green`.

Example:

```json
{
  "lane": 1,
  "time": 0,
  "type": "mood",
  "metas": [
    {
      "key": "bg_color",
      "value": "yellow"
    }
  ],
  "controls": []
}
```

 

*Co-authored with Glean*  
