import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ClickableHeadStates } from '@app/models';


@Component({
    selector: 'app-clickable-head',
    templateUrl: './clickablehead.component.html'
})
export class ClickableHeadComponent {
    @Input() title: string;
    @Input() propertyName: string;
    @Output() stateChanged = new EventEmitter<any>();
    state: ClickableHeadStates = ClickableHeadStates.None;

    constructor() {}

    onSwitchState() {
        if (this.state === ClickableHeadStates.Up) {
            this.state = ClickableHeadStates.None;
        }
        else {
            this.state++;
        }

        this.stateChanged.emit({ state: this.state, name: this.propertyName});
    }
}
