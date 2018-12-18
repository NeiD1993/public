import { ComponentFactoryResolver, EventEmitter, Output, TemplateRef, ViewContainerRef } from '@angular/core';
import { TaskExecutor } from 'src/app/tools/task-executor';

export abstract class BaseIntervalStructuralDirective {

    protected _callbackFunctionParameters: any;

    private _interval: number;

    protected _parameters: any;

    constructor(protected _componentFactoryResolver: ComponentFactoryResolver, protected _template: TemplateRef<any>,
        protected _viewContainer: ViewContainerRef) {
        this._interval = this.setInterval();
    }

    protected abstract callbackFunction(parameters: any): void;

    protected abstract formCallbackFunctionParameters(parameters: any): any;

    protected abstract formParameters(parameters: any): any;

    protected abstract setInterval(): number;

    protected startIntervalScheduler(parameters: any): void {
        this._parameters = this.formParameters(parameters);
        this._callbackFunctionParameters = this.formCallbackFunctionParameters(parameters);
        this.runIntervalScheduler(false);
    }

    protected runIntervalScheduler(immediately: boolean): void {
        let taskExecutor: TaskExecutor = new TaskExecutor();

        taskExecutor.delay = immediately ? 0 : this._interval;
        taskExecutor.start(this.callbackFunction.bind(this, this._callbackFunctionParameters));
    }

    @Output()
    executionComplete: EventEmitter<any> = new EventEmitter<any>();

    @Output()
    positionChanged: EventEmitter<any> = new EventEmitter<any>();
}