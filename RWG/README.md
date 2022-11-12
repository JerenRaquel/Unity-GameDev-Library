# How To Use
1. Create an object and attach RandomWeightedGenerator script to it.
2. Fill out the Weights.
3. Call GenerateValue and recieve the name of the item.

# Example
### Inspector
- Iron
    - Name: Iron
    - Weight: 50
    - Selectable [x]
- Gold
    - Name: Gold
    - Weight: 25
    - Selectable [x]
- Diamond
    - Name: Diamond
    - Weight: 10
    - Selectable [x]

### Code
```csharp
public class FooBar {
  public RandomGenerator.RandomWeightedGenerator rwg;

  public void GetOre() {
    Debug.Log(rwg.GenerateValue())
  }
}
```