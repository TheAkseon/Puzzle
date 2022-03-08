using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NativeGalleryUsage /*: MonoBehaviour*/
{
	public static void PickImage(Action<Texture2D> callback, int maxSize = 1000)
	{
		if (NativeGallery.IsMediaPickerBusy()) return;

		NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
		{
            Debug.Log("Image path: " + path);
            
            if (path != null)
			{
				// Create Texture from selected image
				Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize, false);

				if (texture == null)
				{
					Debug.Log("Couldn't load texture from " + path);					
				}

				callback(texture);
			}
		}, "Select a PNG image", "image/png", maxSize);

		Debug.Log("Permission result: " + permission);
	}
}