import { createColors, RGBA, rgbaString } from 'color-map';
import { ElementRef, Input, Renderer2 } from '@angular/core';

export abstract class BaseColorMapDirective {

    protected _colors: Array<RGBA>;

    constructor(protected _element: ElementRef, protected _renderer: Renderer2) { }

    @Input()
    set colors(colorsMetadata: { startColor: RGBA, endColor: RGBA, colorsCount: number }) {
        this._colors = createColors(colorsMetadata.startColor, colorsMetadata.endColor, colorsMetadata.colorsCount);
    }

    protected getColor(colorIndex: number): string {
        return rgbaString(this._colors[colorIndex]);
    }

    protected setColor(colorIndex: number): void {
        this._renderer.setStyle(this._element.nativeElement, 'color', this.getColor(colorIndex));
    }
}