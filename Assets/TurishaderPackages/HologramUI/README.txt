
UI Hologram VFX - Material Parameters Guide
===========================================

If you experience any issues or have questions, feel free to contact:
Turishader@gmail.com

Color
-----
- ColorMultiplier
  Controls the overall brightness/intensity of the shader. Higher values will result in a more vibrant glow.

Pattern
-------
- PatternSize
  Adjusts the size of the pattern dots or pixels. Affects how fine or coarse the holographic pattern looks.

- Pattern
  Dropdown menu to select the pattern style. Options include:
    * Dots: Circular points
    * Pixels: Square pixels
    * (Additional pattern styles if available)

Mip Level Aura (only in HologramUI.shader)
------------------------------------------
*Requires mipmap-enabled textures*

- MipLevel
  Defines which mipmap level to use for aura sampling. Higher levels result in softer, blurrier edges.

- MipLevelIntensity
  Controls how strongly the mipmap-based Fresnel effect appears. Zero disables it; higher values amplify the effect.

Glitch
------
- GlitchAmount
  Amount of temporal and spatial glitch applied to the hologram. Useful for subtle flicker or more aggressive distortion.

- GlitchMinOpacity
  Sets the minimum opacity during glitching. Lower values make the effect more transparent.

- GlitchTiling
  Tiling factor of the glitch texture. Affects how dense and detailed the distortion appears.

- DistortionAmount
  Intensity of screen-space UV distortion during glitching. Higher values make the effect wobble more aggressively.

Text Shader Only - Text Customization
-------------------------------------
- TextDilate
  Expands the thickness of the text geometry. Can be used to create boldness or glow emphasis.

- TextDilateExtension
  Extends the dilation softly for a glowing fringe effect. Best used in combination with TextDilate.

Render Settings
---------------
- Render Queue
  Controls when the material is rendered relative to others. Default is 3000 (transparent objects).
  Recommended: Leave at "From Shader" unless compositing issues arise.

- Enable GPU Instancing
  Allows Unity to batch multiple instances of the material to improve performance.

- Double Sided Global Illumination
  Enables lighting on both sides of the UI element, mainly relevant if you're using lighting probes or GI.

- Global Illumination
  Specifies how this material participates in global illumination. Usually left as None for UI.
