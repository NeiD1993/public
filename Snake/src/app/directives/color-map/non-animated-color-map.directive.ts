import { BaseColorMapDirective } from "./base-color-map.directive";
import { Directive, ElementRef, Input, Renderer2 } from "@angular/core";

@Directive({
    selector: '[nonAnimatedColorMap]'
})
export class NonAnimatedColorMapDirective extends BaseColorMapDirective {

    constructor(element: ElementRef, renderer: Renderer2) {
        super(element, renderer);
    }

    @Input()
    set nonAnimatedColorMap(colorIndex: number) {
        this._renderer.setStyle(this._element.nativeElement, 'color', this.getColor(colorIndex));
    }
}