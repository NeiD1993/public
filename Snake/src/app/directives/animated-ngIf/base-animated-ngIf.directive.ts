import { AnimationMetadata } from '@angular/animations';
import { AnimationService } from 'src/app/services/animation.service';
import { EmbeddedViewRef, EventEmitter, Output, Renderer2, TemplateRef, ViewContainerRef } from '@angular/core';

export abstract class BaseAnimatedNgIfDirective {

    constructor(private _animationService: AnimationService, protected _renderer: Renderer2, protected _viewContainer: ViewContainerRef) { }

    protected abstract onAnimationDone(parameters?: any): void;

    protected insertTemplate(template?: TemplateRef<any>): EmbeddedViewRef<any> | undefined {
        this._viewContainer.clear();

        return template ? this._viewContainer.createEmbeddedView(template) : undefined;
    }

    protected playAnimation(animation: AnimationMetadata, animationEmbeddedView: EmbeddedViewRef<any>, onAnimationDoneParameters?: any): void {
        this._animationService.playAnimation(animation, this._renderer.parentNode(this._viewContainer.element.nativeElement), () => {
            if (this._viewContainer.indexOf(animationEmbeddedView) != -1)
                this.onAnimationDone.call(this, onAnimationDoneParameters);

            this.animationPlayed.emit();
        });
    }

    @Output()
    animationPlayed: EventEmitter<any> = new EventEmitter();
}