using UnityEngine;
using System.Collections;

public class ColorChange : MonoBehaviour {

	SpriteRenderer sp;

	float red,green,blue;


	float hue=0, saturation=100f, intensity=100f;

	public float alpha=1f;

	public float hueMin = 0, hueMax = 360f;
	public float saturationMin = 0, saturationMax = 100f;
	public float intensityMin = 0, intensityMax = 100f;

	// Use this for initialization
	void Start () {

		hue = Random.Range (hueMin, hueMax);
		saturation = Random.Range (saturationMin, saturationMax);
		intensity = Random.Range (intensityMin, intensityMax);

		HSItoRGB ();

		if (alpha > 1)
			alpha = 1;
		else if (alpha < 0)
			alpha = 0;

		sp = GetComponent<SpriteRenderer> ();
		sp.color = new Color (red,green,blue,alpha);
	}

	void HSItoRGB(){
		
		if (hue > 359)
			hue = 359;
		else if (hue < 0)
			hue = 0;

		if (saturation > 100)
			saturation = 100;
		else if (saturation < 0)
			saturation = 0;

		if (intensity > 100)
			intensity = 100;
		else if (intensity < 0)
			intensity = 0;





		if (saturation == 0) {
			red = intensity;
			green = intensity;
			blue = intensity;
		} else {

			float f, p, q, t;
			int i;

			saturation /= 100;
			intensity /= 100;

			hue /= 60;
			i = (int)hue;
			f = hue - i;

			p = intensity * (1 - saturation);
			q = intensity * (1 - (saturation * f));
			t = intensity * (1 - (saturation * (1 - f)));

			switch(i){
			case 0 : red = intensity;	green = t;			blue = p;			break;
			case 1 : red = q;			green = intensity;	blue = p;			break;
			case 2 : red = p;			green = intensity;	blue = t;			break;
			case 3 : red = p;			green = q;			blue = intensity;	break;
			case 4 : red = t;			green = p;			blue = intensity;	break;
			case 5 : red = intensity;	green = p;			blue = q;			break;
			default : red = 0;			green = 0;			blue = 0;			break;
			}

		}
	}
}
