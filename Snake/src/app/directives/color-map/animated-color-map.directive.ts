import { AnimationMetadata } from "@angular/animations";
import { AnimationService } from "src/app/services/animation.service";
import { BaseColorMapDirective } from "./base-color-map.directive";
import { Directive, ElementRef, Input, Renderer2 } from "@angular/core";

@Directive({
    selector: '[animatedColorMap]'
})
export class AnimatedColorMapDirective extends BaseColorMapDirective {

    constructor(private _animationService: AnimationService, element: ElementRef, renderer: Renderer2) {
        super(element, renderer);
    }

    @Input()
    set animatedColorMap(parameter: { animation: AnimationMetadata, colorIndex: number }) {
        let setColorHandle = () => this.setColor(parameter.colorIndex);

        if (parameter.animation)
            this._animationService.playAnimation(parameter.animation, this._element.nativeElement, setColorHandle);
        else
            setColorHandle();
    }
}