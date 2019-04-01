import { Component, ElementRef, Input, OnInit, OnDestroy, ViewChild, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { DataService } from '../../data.service';

import * as d3 from 'd3';

declare let $: any;

@Component({
    selector: 'map-graph',
    templateUrl: './graph.component.html',
    styleUrls: ['./graphs.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class MapGraphComponent implements OnInit, OnDestroy {

    @ViewChild('container') graphContainer: ElementRef;
    @Input() customBackend: string = null;
    @Input() showPeriodSelection = true;
    @Input()
    set serverID(serverID: number) {
        this.id = serverID;
        this.getActivity();
    }

    data: Array<{ id: number, name: string, count: number}>;
    error = false;
    loading = false;
    notFound = false;
    period = 'day';
    graphTitle = 'Map popularity';

    private id = 0;

    private arc: any;
    private colours: any = d3.scaleOrdinal(d3.schemeCategory10);
    private container: any;
    private graph: any;
    private height: number;
    private legend: { rectSize: number, spacing: number } = { rectSize: 10, spacing: 3 };
    private margin: { top: number, right: number, bottom: number, left: number } = { top: 0, right: 5, bottom: 20, left: 20 };
    private pie: any;
    private tooltip: any;
    private total: number;
    private width: number;

    private ngUnsubscribe = new Subject();

    constructor(private dataService: DataService, private router: Router) { }

    ngOnInit() {
        this.createChart();
        this.getActivity();
        if (this.showPeriodSelection) {
            setTimeout(() => $('ul.tabs').tabs(), 100);
        }
    }

    ngOnDestroy() {
        this.tooltip.remove();
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }

    getActivity(): void {
        if (this.id === null) {
            return;
        }

        let backendPath = this.customBackend;

        if (backendPath === null || backendPath === '') {
            backendPath = this.id !== 0 ? '/servers/' + this.id + '/activity/maps' : '/activity/maps';
        }

        this.loading = true;
        this.dataService.get(backendPath, { period: this.period })
            .pipe(
                takeUntil(this.ngUnsubscribe)
            ).subscribe(data => {
                this.data = data;
                this.error = false;
                this.notFound = false;
                this.loading = false;
                this.updateChart();
            }, err => {
                let errorResponse = err.error;

                if (errorResponse && errorResponse.code === 404) {
                    this.notFound = errorResponse;
                } else {
                    if (errorResponse === null) {
                        errorResponse = {
                            message: 'Unable to retrieve map activity'
                        };
                    }

                    this.error = errorResponse;
                }

                this.loading = false;
                this.container.style('display', 'none');
            });
    }

    selectPeriod(period: string): void {
        if (period === this.period || this.loading) {
            return;
        }

        this.period = period;
        this.getActivity();
    }

    private createChart(): void {
        this.tooltip = d3.select('body').append('div')
            .attr('class', 'tooltip grey darken-4 white-text z-depth-3')
            .style('opacity', 0);

        this.container = d3.select(this.graphContainer.nativeElement);
        const dimensions = this.container.node().getBoundingClientRect();

        this.width = dimensions.width;
        this.height = dimensions.height;

        const radius = Math.min(this.width / 2, this.height) / 2;

        this.graph = this.container.append('svg')
            .attr('width', this.width + this.margin.left + this.margin.right)
            .attr('height', this.height + this.margin.top + this.margin.bottom)
            .append('g');

        this.pie = d3.pie()
            .value((d: any) => d.count);

        this.arc = d3.arc()
            .outerRadius(radius)
            .innerRadius(0);
    }

    private updateChart() {
        this.container.style('display', 'block');

        const self = this;
        this.total = d3.sum(this.data, (d) => d.count);

        const mapDistribution = [];

        let otherCount = 0;
        for (let i = 0; i < this.data.length; i += 1) {
            if ((this.data[i].count / this.total) > 0.01) { // Only show maps that have a playtime greater than 1%
                mapDistribution.push(this.data[i]);
            } else {
                otherCount += this.data[i].count;
            }
        }

        if (otherCount > 0) {
            mapDistribution.push({ name: 'Other', count: otherCount, colour: '#9e9e9e' });
        }

        mapDistribution.sort((a, b) => b.count - a.count);

        this.graph.append('g')
            .attr('class', 'slices');
        this.graph.attr('transform', 'translate(' + (this.width / 4) + ',' + this.height / 2 + ')');

        /* ------- PIE SLICES -------*/
        const slice = this.graph.select('.slices')
            .selectAll('path.slice')
            .data(this.pie(mapDistribution), (d) => d.data.id);

        slice.exit()
            .transition()
            .duration(2000)
            .attrTween('d', function () {
                const exitAngle = this._current.startAngle > (Math.PI / 2) ? 2 * Math.PI : 0;
                const interpolate = d3.interpolate(this._current, { startAngle: exitAngle, endAngle: exitAngle});
                return (t) => self.arc(interpolate(t));
            })
            .remove();

        slice.transition()
            .duration(2000)
            .attrTween('d', function (d) {
                const interpolate = d3.interpolate(this._current, d);
                this._current = d;
                return (t) => self.arc(interpolate(t));
            });

        slice.enter()
            .insert('path')
            .style('fill', (d) => {
                return d.data.colour || this.colours(d.data.id);
            })
            .attr('class', 'slice')
            .on('mouseover', () =>
                this.tooltip.transition()
                    .duration(250)
                    .style('opacity', 0.9))
            .on('mousemove', (d) => {
                this.tooltip.html(d.data.name + '<br>' + Math.round((d.data.count / this.total) * 100) + '%')
                    .style('left', d3.event.pageX + 15 + 'px')
                    .style('top', d3.event.pageY - 25 + 'px');
            })
            .on('mouseout', () => {
                this.tooltip.transition()
                    .duration(500)
                    .style('opacity', 0);
            })
            .transition()
            .duration(2000)
            .attrTween('d', function (d) {
                this._current = d;
                const entryAngle = this._current.startAngle > (Math.PI / 2) ? 2 * Math.PI : 0;
                const interpolate = d3.interpolate({ startAngle: entryAngle, endAngle: entryAngle }, d);
                return (t) => self.arc(interpolate(t));
            });

        /* ------- LEGEND -------*/
        this.graph.select('.legend').remove();
        const legend = this.graph
            .append('g')
            .attr('class', 'legend')
            .selectAll('.legend')
            .data(mapDistribution)
            .enter()
            .append('g')
            .attr('transform', (d, i) => {
                const height = this.legend.rectSize + this.legend.spacing;
                const horz = this.width / 4 + 15;
                const vert = i * height - (height * mapDistribution.length) / 2;
                return 'translate(' + horz + ',' + vert + ')';
            });
        legend.append('rect')
            .attr('width', this.legend.rectSize)
            .attr('height', this.legend.rectSize)
            .style('fill', (d) => d.colour || this.colours(d.id))
            .style('stroke', (d) => d.colour || this.colours(d.id));
        legend.append('a')
            .attr('xlink:href', (d) => {
                if (d.name === 'Other') {
                    return '#';
                }
                return '/maps/' + d.id;
            })
            .append('text')
            .attr('x', this.legend.rectSize + this.legend.spacing)
            .attr('y', this.legend.rectSize)
            .text((d) => d.name)
            .style('fill', '#039be5')
            .on('click', (d) => {
                d3.event.preventDefault();
                if (d.name !== 'Other') {
                    this.router.navigate(['/maps', d.id]);
                }
            });
    }
}
