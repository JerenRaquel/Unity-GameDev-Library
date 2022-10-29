# How to Use
- [**Note**] Requires TextMeshPro.
- TextScroller.Skip() will display the final text without having to wait the scrolling.

1. Create FontData to use for portions of text.
2. Create and assign a default FontData for the TextScroller to use.
3. Create TextData for your chunk of text.
    - This chunk will be the entire text displayed in the TMP textbox.
    - [**Note**] Leave the FontData field blank for default FontData.
4. Call TextScroller.Read() to start the character by character apperance.


# Example
```csharp
// Some portion of code in a controller or similar
public class FooBar : Monobehavior {
  public TextScroller textScroller;
  ...
  public void PlayNextDialogue() {
    textScroller.Read();
  }
}
```