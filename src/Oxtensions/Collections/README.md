# Collections Extensions

> `using Oxtensions.Collections;`

Extension methods for `System.Collections.Generic` types: `IList<T>`, `IDictionary<TKey, TValue>`, `ISet<T>`, `IReadOnlyList<T>`, `IReadOnlyDictionary<TKey, TValue>`, `Stack<T>`, and `Queue<T>`.

---

## ListExtensions

| Method | Description |
|--------|-------------|
| `IsNullOrEmpty()` | Null or zero-count check |
| `Chunk(size)` | Split into fixed-size segments |
| `Shuffle()` | Fisher-Yates in-place shuffle |
| `DistinctBy(keySelector)` | Deduplicate by key |
| `ForEach(action)` | Side-effect iteration |
| `ToHashSet(keySelector)` | Project to `HashSet<TKey>` |
| `Paginate(page, pageSize)` | Skip/Take pagination |
| `AddRangeIfNotExists(items)` | Add only missing items |
| `RemoveWhere(predicate)` | Remove matching elements in-place |
| `MinBy(selector)` | Element with minimum projected value |
| `MaxBy(selector)` | Element with maximum projected value |
| `Flatten()` | Flatten nested `IEnumerable<IEnumerable<T>>` into a single sequence |
| `RandomItem()` | Returns a random element from the list |
| `RandomItems(count)` | Returns `count` distinct random elements |
| `Rotate(count)` | Rotates elements in-place (positive = left, negative = right) |
| `Interleave(second)` | Interleaves two sequences element-by-element |
| `CountBy(keySelector)` | Groups and counts elements by key — returns `Dictionary<TKey, int>` |
| `None(predicate?)` | `true` when no element satisfies the predicate (or list is empty) |
| `HasDuplicates()` | `true` when at least two elements are equal |
| `Duplicates(keySelector)` | Returns only the duplicate elements by key |

> **Note — .NET 6+ name conflicts:** `DistinctBy`, `ToHashSet`, `MinBy`, and `MaxBy` also exist in `System.Linq.Enumerable`. Call them as static methods to use the Oxtensions versions explicitly:
> ```csharp
> ListExtensions.DistinctBy(myList, x => x.Id);
> ```

---

## DictionaryExtensions

| Method | Description |
|--------|-------------|
| `IsNullOrEmpty()` | Null or zero-count check |
| `GetOrDefault(key, defaultValue?)` | Safe lookup — returns fallback when key is missing |
| `GetOrAdd(key, valueFactory)` | Returns existing value or inserts a new one |
| `Merge(other, overwrite?)` | Merges another dictionary, optionally overwriting |
| `ToQueryString()` | Converts to URL query string (e.g. `?a=1&b=2`) |
| `Invert()` | Swaps keys and values |
| `AddOrUpdate(key, value)` | Inserts if absent, updates if present |
| `RemoveWhere(predicate)` | Removes all pairs matching a predicate; returns count |
| `ToJson()` | Serialises to JSON string |

---

## StackExtensions

| Method | Description |
|--------|-------------|
| `IsNullOrEmpty()` | `true` when the stack is `null` or empty |
| `PeekOrDefault(defaultValue?)` | Returns the top element without removing it, or a fallback when empty |
| `PopOrDefault(defaultValue?)` | Removes and returns the top element, or a fallback when empty |
| `PushRange(items)` | Pushes all items in enumeration order (last item ends up on top) |
| `PopMany(count)` | Pops up to `count` items and returns them as `T[]` |
| `Clone()` | Returns a shallow copy of the stack with identical element order |

---

## QueueExtensions

| Method | Description |
|--------|-------------|
| `IsNullOrEmpty()` | `true` when the queue is `null` or empty |
| `PeekOrDefault(defaultValue?)` | Returns the front element without removing it, or a fallback when empty |
| `DequeueOrDefault(defaultValue?)` | Removes and returns the front element, or a fallback when empty |
| `EnqueueRange(items)` | Enqueues all items in enumeration order |
| `DequeueMany(count)` | Dequeues up to `count` items and returns them as `T[]` |
| `Clone()` | Returns a shallow copy of the queue with identical element order |

---

## Usage Examples

### ListExtensions

```csharp
// Chunk
Enumerable.Range(1, 5).ToList().Chunk(2); // [[1,2],[3,4],[5]]

// Paginate
items.Paginate(page: 0, pageSize: 10);

// Shuffle (in-place)
myList.Shuffle();

// DistinctBy — explicit call to avoid LINQ ambiguity on .NET 6+
ListExtensions.DistinctBy(products, p => p.Id);

// RemoveWhere
myList.RemoveWhere(x => x < 0); // removes all negatives in-place

// ForEach
new[] { 1, 2, 3 }.ForEach(Console.WriteLine);

// ToHashSet
ListExtensions.ToHashSet(new[] { 1, 1, 2, 3, 3 }); // { 1, 2, 3 }

// AddRangeIfNotExists
var list = new List<int> { 1, 2 };
list.AddRangeIfNotExists(new[] { 2, 3, 4 }); // list = [1, 2, 3, 4]

// MinBy / MaxBy
var items = new[] { (Id: 3, Name: "c"), (Id: 1, Name: "a"), (Id: 2, Name: "b") };
ListExtensions.MinBy(items, x => x.Id); // (1, "a")
ListExtensions.MaxBy(items, x => x.Id); // (3, "c")

// Flatten
new[] { new[] { 1, 2 }, new[] { 3, 4 } }.Flatten(); // [1, 2, 3, 4]

// RandomItem / RandomItems
new[] { 1, 2, 3, 4, 5 }.RandomItem();     // one of: 1, 2, 3, 4, 5
new[] { 1, 2, 3, 4, 5 }.RandomItems(3);  // 3 distinct elements

// Rotate (in-place)
var rot = new List<int> { 1, 2, 3, 4, 5 };
rot.Rotate(2);   // [3, 4, 5, 1, 2]
rot.Rotate(-1);  // [2, 3, 4, 5, 1]

// Interleave
new[] { 1, 2, 3 }.Interleave(new[] { 10, 20, 30 }); // [1, 10, 2, 20, 3, 30]

// CountBy
new[] { "a", "bb", "cc", "d" }.CountBy(w => w.Length);
// { 1 => 2, 2 => 2 }

// None
new[] { 1, 2, 3 }.None(x => x < 0);  // true  (no negatives)
new[] { 1, -2, 3 }.None(x => x < 0); // false (-2 exists)

// HasDuplicates
new[] { 1, 2, 2, 3 }.HasDuplicates(); // true
new[] { 1, 2, 3 }.HasDuplicates();    // false

// Duplicates
new[] { 1, 2, 2, 3, 3, 3 }.Duplicates(x => x); // [2, 3]
```

### DictionaryExtensions

```csharp
// Safe lookup
dict.GetOrDefault("key", "fallback"); // "fallback" if missing

// Lazy insertion
cache.GetOrAdd("key", () => ComputeExpensiveValue());

// Merge
base.Merge(overrides, overwrite: true);

// Query string
new Dictionary<string, string> { {"q","hello"}, {"page","1"} }
    .ToQueryString(); // "q=hello&page=1"

// Invert
var byValue = dict.Invert(); // values become keys

// RemoveWhere
int removed = dict.RemoveWhere((k, v) => v < 0); // returns count

// IsNullOrEmpty
((IDictionary<string, int>?)null).IsNullOrEmpty(); // true
new Dictionary<string, int>().IsNullOrEmpty();     // true
new Dictionary<string, int> { ["a"] = 1 }.IsNullOrEmpty(); // false

// AddOrUpdate
dict.AddOrUpdate("newKey", 42);  // inserts when absent
dict.AddOrUpdate("newKey", 99);  // updates when present

// ToJson
new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 }.ToJson();
// {"a":1,"b":2}
```

### StackExtensions

```csharp
var stack = new Stack<int>();
stack.PushRange(new[] { 1, 2, 3 }); // top = 3

stack.PeekOrDefault(-1);            // 3 (no remove)
stack.PopMany(2);                   // [3, 2] — count becomes 1
stack.Clone();                      // independent copy, same top
```

### QueueExtensions

```csharp
var queue = new Queue<string>();
queue.EnqueueRange(new[] { "a", "b", "c" });

queue.PeekOrDefault("(empty)");  // "a" (no remove)
queue.DequeueMany(2);            // ["a", "b"] — count becomes 1
queue.Clone();                   // independent copy, same front
```


---

## StackExtensions

| Method | Description |
|--------|-------------|
| `IsNullOrEmpty()` | `true` when the stack is `null` or empty |
| `PeekOrDefault(defaultValue?)` | Returns the top element without removing it, or a fallback when empty |
| `PopOrDefault(defaultValue?)` | Removes and returns the top element, or a fallback when empty |
| `PushRange(items)` | Pushes all items in enumeration order (last item ends up on top) |
| `PopMany(count)` | Pops up to `count` items and returns them as `T[]` |
| `Clone()` | Returns a shallow copy of the stack with identical element order |

---

## QueueExtensions

| Method | Description |
|--------|-------------|
| `IsNullOrEmpty()` | `true` when the queue is `null` or empty |
| `PeekOrDefault(defaultValue?)` | Returns the front element without removing it, or a fallback when empty |
| `DequeueOrDefault(defaultValue?)` | Removes and returns the front element, or a fallback when empty |
| `EnqueueRange(items)` | Enqueues all items in enumeration order |
| `DequeueMany(count)` | Dequeues up to `count` items and returns them as `T[]` |
| `Clone()` | Returns a shallow copy of the queue with identical element order |

---

## Usage Examples

### IsNullOrEmpty

```csharp
Stack<int>? stack = null;
stack.IsNullOrEmpty();             // true

new Stack<int>().IsNullOrEmpty();  // true

var s = new Stack<int>(new[] { 1 });
s.IsNullOrEmpty();                 // false
```

### PeekOrDefault / PopOrDefault

```csharp
var stack = new Stack<string>();

// Safe peek — no InvalidOperationException
string? top = stack.PeekOrDefault("(empty)"); // "(empty)"

stack.Push("hello");
stack.PopOrDefault();   // "hello" — count becomes 0
stack.PopOrDefault();   // default(string) = null
```

### PushRange

```csharp
var stack = new Stack<int>();
stack.PushRange(new[] { 1, 2, 3 });
// Push order: 1, then 2, then 3 → top = 3

stack.Pop(); // 3
stack.Pop(); // 2
stack.Pop(); // 1
```

### PopMany

```csharp
var stack = new Stack<int>(new[] { 3, 2, 1 }); // top = 1

int[] popped = stack.PopMany(2); // [1, 2]
stack.Count;                     // 1 (only 3 left)

// Safe over-count — returns whatever is available
stack.PopMany(100); // [3]
```

### Clone (Stack)

```csharp
var original = new Stack<int>(new[] { 3, 2, 1 }); // top = 1
var copy = original.Clone();

copy.Push(99); // does not affect original
original.Count; // 3
copy.Count;     // 4
```

---

### PeekOrDefault / DequeueOrDefault

```csharp
var queue = new Queue<string>();

string? front = queue.PeekOrDefault("(empty)"); // "(empty)"

queue.Enqueue("first");
queue.DequeueOrDefault(); // "first"
queue.DequeueOrDefault(); // null (default)
```

### EnqueueRange

```csharp
var queue = new Queue<int>();
queue.EnqueueRange(new[] { 10, 20, 30 });
// Enqueue order preserved → front = 10

queue.Dequeue(); // 10
queue.Dequeue(); // 20
queue.Dequeue(); // 30
```

### DequeueMany

```csharp
var queue = new Queue<int>(new[] { 1, 2, 3, 4 });

int[] batch = queue.DequeueMany(3); // [1, 2, 3]
queue.Count;                        // 1

// Safe over-count
queue.DequeueMany(100); // [4]
```

### Clone (Queue)

```csharp
var original = new Queue<int>(new[] { 1, 2, 3 });
var copy = original.Clone();

copy.Enqueue(99);
original.Count; // 3
copy.Count;     // 4
```

---

## SetExtensions

Extension methods for `ISet<T>` (e.g. `HashSet<T>`, `SortedSet<T>`).

| Method | Description |
|--------|-------------|
| `IsNullOrEmpty()` | `true` when the set is `null` or empty |
| `AddRange(items)` | Adds all items to the set; returns the count of newly inserted elements |
| `RemoveWhere(predicate)` | Removes all elements matching the predicate; returns the count removed |
| `ToSortedSet()` | Creates a new `SortedSet<T>` from the set elements (ascending order) |
| `OverlapsWith(other)` | `true` when the set shares at least one element with `other` |
| `SymmetricDifference(other)` | Returns a new `HashSet<T>` with elements exclusive to each collection (non-mutating) |

### SetExtensions Usage Examples

```csharp
var set = new HashSet<int> { 1, 2, 3 };

// AddRange — returns count of truly new elements
int added = set.AddRange(new[] { 2, 3, 4, 5 }); // added = 2

// RemoveWhere
int removed = set.RemoveWhere(x => x % 2 == 0); // removed = 2 (4 removed)

// ToSortedSet — returns a new sorted copy, source unchanged
SortedSet<int> sorted = new HashSet<int> { 5, 1, 3 }.ToSortedSet(); // { 1, 3, 5 }

// OverlapsWith
new HashSet<int> { 1, 2, 3 }.OverlapsWith(new[] { 3, 4, 5 }); // true

// SymmetricDifference (non-mutating)
new HashSet<int> { 1, 2, 3 }.SymmetricDifference(new[] { 2, 3, 4 }); // { 1, 4 }
```

---

## ReadOnlyListExtensions

Extension methods for `IReadOnlyList<T>` (e.g. arrays, `List<T>`, `ImmutableList<T>`).

| Method | Description |
|--------|-------------|
| `IsNullOrEmpty()` | `true` when the list is `null` or empty |
| `IndexOf(item)` | Zero-based index of the first occurrence; `-1` when not found |
| `LastIndexOf(item)` | Zero-based index of the last occurrence; `-1` when not found |
| `BinarySearch(value)` | Binary search on a sorted list; returns index or `~insertionPoint` |
| `Slice(start, length)` | Returns a read-only sub-list view of the given range |


### ReadOnlyListExtensions Usage Examples

```csharp
IReadOnlyList<int> list = new[] { 10, 20, 30, 10 };

list.IsNullOrEmpty();       // false
list.IndexOf(10);           // 0
list.LastIndexOf(10);       // 3
list.IndexOf(99);           // -1

// BinarySearch (list must be sorted)
IReadOnlyList<int> sorted = new[] { 1, 3, 5, 7, 9 };
sorted.BinarySearch(5);     // 2
sorted.BinarySearch(4);     // negative (~2 = insertion point 2)

// Slice
list.Slice(1, 2); // [20, 30]
```

---

## ReadOnlyDictionaryExtensions

Extension methods for `IReadOnlyDictionary<TKey, TValue>`.

| Method | Description |
|--------|-------------|
| `ContainsAllKeys(keys)` | `true` only when every supplied key is present |
| `ContainsAnyKey(keys)` | `true` when at least one supplied key is present |
| `ToDictionary()` | Creates a mutable `Dictionary<TKey, TValue>` copy |
| `FilterByKeys(keys)` | Returns a new dictionary containing only entries whose keys appear in `keys` |

### ReadOnlyDictionaryExtensions Usage Examples

```csharp
IReadOnlyDictionary<string, int> scores = new Dictionary<string, int>
{
    ["Alice"] = 90, ["Bob"] = 80, ["Carol"] = 70
};

scores.ContainsAllKeys(new[] { "Alice", "Bob" });    // true
scores.ContainsAnyKey(new[] { "Dave", "Carol" });    // true

// Mutable copy — original unchanged
var mutable = scores.ToDictionary();
mutable["Alice"] = 95;

// Filtered copy
scores.FilterByKeys(new[] { "Alice", "Carol" });
// { "Alice" => 90, "Carol" => 70 }
```
