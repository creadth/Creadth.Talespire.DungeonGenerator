import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {MatButtonModule} from '@angular/material/button';
import {MatCardModule} from '@angular/material/card';
import {MatInputModule} from '@angular/material/input';
import {RouterModule} from '@angular/router';
import {FlexModule} from '@angular/flex-layout';
import {ReactiveFormsModule} from '@angular/forms';
import {MatToolbarModule} from '@angular/material/toolbar';
import {ImportDungeonComponent} from './import-dungeon/import-dungeon.component';
import {ClipboardModule} from 'ngx-clipboard';


@NgModule({
    declarations: [ImportDungeonComponent],
    imports: [
        CommonModule,
        RouterModule.forChild([
            {
                path: 'slab',
                children: [
                    {
                        path: 'import-dungeon',
                        component: ImportDungeonComponent
                    }
                ]
            }
        ]),
        MatCardModule,
        MatInputModule,
        FlexModule,
        ReactiveFormsModule,
        MatButtonModule,
        MatToolbarModule,
        ClipboardModule
    ]
})
export class SlabModule {
}
