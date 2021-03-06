//-----------------------------------------------------------------------------
// Copyright (c) 2013 Developer
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------

/// @class AnimationPreviewWindow
/// This class is a SceneWindow that displays a single animation. After
/// creating a SceneWindow and assigning it this class, call
/// AnimationPreviewWindow::display, passing it the animation you would like to
/// display.
/// 
/// @field scene The scene that renders the preview.
/// @field sprite The animated sprite that renders the animation.
/// @field baseDimension The length of the shorter side of the camera.
/// @field animation The animation datablock being previewed.

/// void(AnimationPreviewWindow this)
/// Initializes various properties necessary for the window.
/// @param this The AnimationPreviewWindow.
function AnimationPreviewWindow::onAdd(%this)
{
    Assert(%this.getClassName() $= "SceneWindow", "AnimationPreviewWindow::onAdd - This is not a SceneWindow!");

    %this.scene = new Scene();
    %this.sprite = new sprite();
    %this.sprite.scene = %this.scene;
    %this.baseDimension = 100;
    %this.animation = "";

    %this.setScene(%this.scene);
}

/// void(AnimationPreviewWindow this)
/// Cleans up data allocated for this window.
/// @param this The AnimationPreviewWindow.
function AnimationPreviewWindow::onRemove(%this)
{
    if (isObject(%this.scene))
        %this.scene.delete();
      
    if (isObject(%this.sprite))
        %this.sprite.delete();
}

/// void(AnimationPreviewWindow this)
/// Called when the window is first shown to make sure things are properly
/// sized.
/// @param this The AnimationPreviewWindow.
function AnimationPreviewWindow::onWake(%this)
{
    %this.onExtentChange(%this.position SPC %this.extent);
}

/// void(AnimationPreviewWindow this, RectF newDimensions)
/// Resizes the scenes camera to maintain the correct aspect ratio.
/// @param this The AnimationPreviewWindow.
/// @param newDimensions The new position and size of the window.
function AnimationPreviewWindow::onExtentChange(%this, %newDimensions)
{
    %width = getWord(%newDimensions, 2);
    %height = getWord(%newDimensions, 3);

    %dimensions = resolveAspectRatio(%width / %height, %this.baseDimension);
    %cameraWidth = getWord(%dimensions, 0);
    %cameraHeight = getWord(%dimensions, 1);

    %this.setCurrentCameraPosition(0, 0, %cameraWidth, %cameraHeight);
}

/// void(AnimationPreviewWindow this, ImageAsset imageAsset)
/// Displays the specified image map in this window.
/// @param this The AnimationPreviewWindow.
/// @param animation The animation to display.
function AnimationPreviewWindow::display(%this, %animation)
{
    Assert(!AssetDatabase.isDeclaredAsset(%animation), "AnimationPreviewWindow::display - Object is not an animation!");

    %this.animation = %animation;
    %this.update();
}

/// void(AnimationPreviewWindow this)
/// Clears the display.
/// @param this The AnimationPreviewWindow.
function AnimationPreviewWindow::clear(%this)
{
    %this.animation = "";
    %this.update();
}

/// void(AnimationPreviewWindow this)
/// Updates the displayed animation.
/// @param this The AnimationPreviewWindow.
/// @todo This doesn't take advantage of width or height when displaying
/// non-square animations.
function AnimationPreviewWindow::update(%this)
{
    if (!isObject(%this.animation))
    {
        %this.sprite.setVisible(false);
        return;
    }

    %this.sprite.setVisible(true);

    %imageAsset = AssetDatabase.acquireAsset(AnimationBuilder.sourceImageAsset);

    // Frame size.
    %size = %imageAsset.getFrameSize(0);

    AssetDatabase.releaseAsset(%imageAsset.getAssetId());

    %width = getWord(%size, 0);
    %height = getWord(%size, 1);

    // Camera size.
    %cameraSize = %this.getCurrentCameraSize();
    %cameraWidth = getWord(%cameraSize, 0);
    %cameraHeight = getWord(%cameraSize, 1);

    // Determine the amount to scale the sprite so it fills the camera.
    %widthScale = %cameraWidth / %width;
    %heightScale = %cameraHeight / %height;
    %scale = %widthScale < %heightScale ? %widthScale : %heightScale;

    %width *= %scale;
    %height *= %scale;

    %this.sprite.size = %width SPC %height;
    %this.sprite.playAnimation(%this.animation);
}

/// void(AnimationPreviewWindow this)
/// Pauses the animation that is playing.
/// @param this The AnimationPreviewWindow.
function AnimationPreviewWindow::pause(%this)
{
    if (isObject(%this.animation))
    {
        %this.sprite.pauseAnimation(true);
        %this.sprite.paused = true;
    }
}

/// void(AnimationPreviewWindow this)
/// Plays the animation.
/// @param this The AnimationPreviewWindow.
function AnimationPreviewWindow::play(%this)
{
    if (isObject(%this.animation))
    {
        if (%this.sprite.paused)
        {
            %this.sprite.pauseAnimation(false);
            %this.sprite.paused = false;
        }
        else
        {
            %this.sprite.pauseAnimation(false);
            %this.sprite.paused = false;
            %this.sprite.playAnimation(%this.animation.getAssetId());
        }
    }
}
