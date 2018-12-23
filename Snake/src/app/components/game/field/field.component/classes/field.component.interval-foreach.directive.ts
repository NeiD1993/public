import { BaseIntervalStructuralDirective } from 'src/app/directives/base-interval-structural.directive';
import { BrickComponent } from 'src/app/components/game/field/brick.component/classes/brick.component';
import { Bricks } from 'src/app/models/bricks';
import { ComponentFactoryResolver, ComponentRef, Directive, Inject, InjectionToken, Input, OnDestroy, TemplateRef, ViewContainerRef } from '@angular/core';
import { createSubscriptionServiceFactoryProvider, SubscriptionsService } from 'src/app/services/subscriptions.service';
import { FieldComponent } from 'src/app/components/game/field/field.component/classes/field.component';

let fieldComponentLoadingContinuedEventSubscriptionsServiceToken: InjectionToken<any> =
    new InjectionToken('FieldComponentLoadingContinuedEventSubscriptionsService');

@Directive({
    selector: '[fieldIntervalForeach]',
    providers: [createSubscriptionServiceFactoryProvider<FieldComponent>(fieldComponentLoadingContinuedEventSubscriptionsServiceToken)]
})
export class FieldComponentIntervalForeachDirective extends BaseIntervalStructuralDirective implements OnDestroy {

    constructor(componentFactoryResolver: ComponentFactoryResolver, template: TemplateRef<any>,
        viewContainer: ViewContainerRef, fieldComponent: FieldComponent, @Inject(fieldComponentLoadingContinuedEventSubscriptionsServiceToken)
        private _fieldComponentLoadingContinuedEventSubscriptionsServiceToken: SubscriptionsService<FieldComponent>) {
        super(componentFactoryResolver, template, viewContainer);
        this._fieldComponentLoadingContinuedEventSubscriptionsServiceToken.addSubscription(fieldComponent,
            fieldComponent.loadingContinued.subscribe(this.onFieldComponentLoadingContinued.bind(this)));
    }

    @Input()
    set fieldIntervalForeach(parameters: { condition: boolean, bricks: Bricks }) {
        if (parameters.condition) {
            this._viewContainer.clear();
            this.startIntervalScheduler(parameters.bricks);
        }
    }

    private insertBrickComponent(indexes: { row: number, column: number }, bricks: Bricks): BrickComponent {
        let brickComponent: ComponentRef<BrickComponent> =
            this._viewContainer.createComponent(this._componentFactoryResolver.resolveComponentFactory(BrickComponent));

        brickComponent.instance.brick = bricks.elements[indexes.row][indexes.column];
        this._template.createEmbeddedView(brickComponent.location.nativeElement);

        return brickComponent.instance;
    }

    protected callbackFunction(parameters: any): void {
        let bricksParameters = <Bricks>parameters;
        let concreteParameters = <{ positionIndex: number, embeddedBrickComponents: Array<BrickComponent> }>this._parameters;
        let columnIndex = concreteParameters.positionIndex;

        if (columnIndex < bricksParameters.sideBricksCount) {
            for (let rowIndex = 0; rowIndex < bricksParameters.sideBricksCount; rowIndex++)
                concreteParameters.embeddedBrickComponents.push(this.insertBrickComponent({ row: rowIndex, column: columnIndex }, bricksParameters));

            this.positionChanged.emit(++concreteParameters.positionIndex);
        }
        else
            this.executionComplete.emit(concreteParameters.embeddedBrickComponents);
    }

    protected formParameters(): any {
        return { positionIndex: 0, embeddedBrickComponents: new Array<BrickComponent>() };
    }

    protected setInterval(): number {
        return 124;
    }

    ngOnDestroy(): void {
        this._fieldComponentLoadingContinuedEventSubscriptionsServiceToken.resetSubscriptions();
    }

    onFieldComponentLoadingContinued(): void {
        this.runIntervalScheduler(!((<{ positionIndex: number, embeddedBrickComponents: Array<BrickComponent> }>this._parameters).positionIndex < 100));
    }
}