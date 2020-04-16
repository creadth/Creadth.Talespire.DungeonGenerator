import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ParseSlabComponent} from './parse-slab/parse-slab.component';
import {RouterModule} from '@angular/router';
import {MatCardModule} from '@angular/material/card';
import {MatInputModule} from '@angular/material/input';
import {FlexModule} from '@angular/flex-layout';
import {ReactiveFormsModule} from '@angular/forms';
import {MatButtonModule} from '@angular/material/button';
import {MatToolbarModule} from '@angular/material/toolbar';
import {ImportDungeonComponent} from './import-dungeon/import-dungeon.component';
import {ClipboardModule} from 'ngx-clipboard';
import {Json2SlabComponent} from './json-2-slab/json-2-slab.component';


@NgModule({
    declarations: [ParseSlabComponent, ImportDungeonComponent, Json2SlabComponent],
    imports: [
        CommonModule,
        RouterModule.forChild([
            {
                path: 'slab',
                children: [
                    {
                        path: 's2j',
                        component: ParseSlabComponent
                    },
                    {
                        path: 'j2s',
                        component: Json2SlabComponent
                    },
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
