import { AnimationMetadata } from '@angular/animations';
import { AnimationService } from 'src/app/services/animation.service';
import { BaseAnimatedNgIfDirective } from './base-animated-ngIf.directive';
import { Directive, Input, Renderer2, TemplateRef, ViewContainerRef } from '@angular/core';

@Directive({
    selector: '[animatedBeforeNgIf]'
})
export class AnimatedBeforeNgIfDirective extends BaseAnimatedNgIfDirective {

    constructor(animationService: AnimationService, renderer: Renderer2, viewContainer: ViewContainerRef) {
        super(animationService, renderer, viewContainer);
    }

    @Input()
    set animatedBeforeNgIf(parameter: { animation: AnimationMetadata, template: TemplateRef<any> }) {
        if (!parameter.animation)
            this.insertTemplate(parameter.template);
        else
            this.playAnimation(parameter.animation, parameter.template);
    }

    protected onAnimationDone(parameters: any): void {
        this.insertTemplate(<TemplateRef<any>>parameters);
    }
}