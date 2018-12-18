import { AnimationMetadata } from '@angular/animations';
import { AnimationService } from 'src/app/services/animation.service';
import { BaseAnimatedNgIfDirective } from './base-animated-ngIf.directive';
import { Directive, Input, Renderer2, TemplateRef, ViewContainerRef } from '@angular/core';

@Directive({
    selector: '[animatedAfterNgIf]'
})
export class AnimatedAfterNgIfDirective extends BaseAnimatedNgIfDirective {

    constructor(animationService: AnimationService, renderer: Renderer2, viewContainer: ViewContainerRef) {
        super(animationService, renderer, viewContainer);
    }

    @Input()
    set animatedAfterNgIf(parameter: { animation: AnimationMetadata, template: TemplateRef<any> } | undefined) {
        if (parameter)
            this.playAnimation(parameter.animation, this.insertTemplate(parameter.template));
    }

    protected onAnimationDone(): void {
        this.insertTemplate();
    }
}