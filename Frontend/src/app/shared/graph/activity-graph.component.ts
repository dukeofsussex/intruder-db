// Original at http://bl.ocks.org/d3noob/6eb506b129f585ce5c8a
import { Component, ElementRef, Input, OnInit, OnDestroy, ViewChild, ViewEncapsulation } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { DataService } from '../../data.service';

import * as d3 from 'd3';

declare let $: any;

@Component({
    selector: 'activity-graph',
    templateUrl: './graph.component.html',
    styleUrls: ['./graphs.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class ActivityGraphComponent implements OnInit, OnDestroy {

    @ViewChild('container') graphContainer: ElementRef;
    @Input() customBackend: string = null;
    @Input() customTimeFormat: string = null;
    @Input() showPeriodSelection = true;
    @Input() period = 'day';
    @Input() graphTitle = 'Recent activity';
    @Input()
    set serverID(serverID: number) {
        this.id = serverID;
        this.getActivity();
    }

    data: Array<{ agents: number, timestamp: string }>;
    error = false;
    loading = false;
    notFound = false;

    private id = 0;

    private container: any;
    private dimensions: any;
    private focus: any;
    private graph: any;
    private height: number;
    private line: any;
    private margin: { top: number, right: number, bottom: number, left: number } = { top: 5, right: 15, bottom: 30, left: 30 };
    private ticks = 5;
    private tooltip: any;
    private width: number;
    private x: any;
    private xAxis: any;
    private y: any;
    private yAxis: any;

    private ngUnsubscribe = new Subject();

    constructor(private dataService: DataService) { }

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
            backendPath = this.id !== 0 ? '/servers/' + this.id + '/activity' : '/activity';
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
                            message: 'Unable to retrieve activity'
                        };
                    }

                    this.error = errorResponse;
                }

                this.loading = false;
                this.container.style('display', 'none');
            });
    }

    selectPeriod(period: string): void {
        if (period === this.period) {
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
        this.dimensions = this.container.node().getBoundingClientRect();

        this.width = this.dimensions.width - this.margin.left - this.margin.right;
        this.height = this.dimensions.height - this.margin.top - this.margin.bottom;

        this.x = d3.scaleTime().range([0, this.width]);
        this.y = d3.scaleLinear().range([this.height, 0]);

        this.line = d3.line()
            .x((d: any) => {
                return this.x(d.timestamp);
            })
            .y((d: any) => {
                return this.y(d.agents);
            });

        this.graph = this.container.append('svg')
            .attr('width', this.width + this.margin.left + this.margin.right)
            .attr('height', this.height + this.margin.top + this.margin.bottom)
            .append('g')
            .attr('transform', 'translate(' + this.margin.left + ',' + this.margin.top + ')');

        this.xAxis = this.graph.append('g')
            .attr('class', 'axis')
            .attr('transform', 'translate(0,' + this.height + ')');

        this.xAxis.append('text')
            .attr('class', 'axis-title')
            .attr('x', this.width / 2)
            .attr('y', 30)
            .style('font-size', '11px')
            .style('text-anchor', 'middle')
            .attr('fill', '#757575')
            .text('Timezone: CET/CEST');

        this.yAxis = this.graph.append('g')
            .attr('class', 'axis');

        this.focus = this.graph.append('g')
            .attr('class', 'focus')
            .style('display', 'none');
    }

    private getTimeFormat(showDetailedTimestamp: boolean) {
        let timeFormat: string;

        if (this.customTimeFormat !== null) {
            return this.customTimeFormat;
        }

        switch (this.period) {
            case 'month':
                timeFormat = (showDetailedTimestamp ? '%A ' : '') + '%d %B' + (showDetailedTimestamp ? ' %H:%M' : '');
                break;
            case 'week':
                timeFormat = (showDetailedTimestamp ? '%A ' : '') + '%d %B' + (showDetailedTimestamp ? ' %H:%M' : '');
                break;
            case 'day':
            default:
                timeFormat = (showDetailedTimestamp ? '%d %B ' : '') + '%H:%M';
                break;
        }

        return timeFormat;
    }

    private updateChart() {
        const self = this;
        this.container.style('display', 'block');

        this.data.forEach((d: any) => {
            d.timestamp = d3.timeParse('%Y-%m-%dT%H:%M:%S')(d.timestamp);
        });

        this.x.domain(d3.extent(this.data, (d: any) => {
            return d.timestamp;
        }));

        this.y.domain([0, d3.max(this.data, (d: any) => {
            return d.agents;
        }) * 1.05]);

        this.xAxis.call(d3.axisBottom(this.x).ticks(this.ticks).tickFormat((d: any) => {
            return d3.timeFormat(this.getTimeFormat(false))(d);
        }));

        this.yAxis.call(d3.axisLeft(this.y).ticks(this.ticks).tickFormat((d: any) => {
            return d;
        }));

        this.graph.selectAll('path.line').remove();
        this.graph.append('path')
            .datum(this.data)
            .attr('class', 'line')
            .attr('d', this.line);

        this.focus.selectAll('*').remove();

        this.focus.append('line')
            .attr('class', 'x')
            .style('stroke', '#263238')
            .style('stroke-dasharray', '3,3')
            .style('opacity', 0.75)
            .attr('y1', 0)
            .attr('y2', this.height);

        this.focus.append('line')
            .attr('class', 'y')
            .style('stroke', '#263238')
            .style('stroke-dasharray', '3,3')
            .style('opacity', 0.75)
            .attr('x1', this.width)
            .attr('x2', this.width);

        this.focus.append('circle')
            .attr('class', 'y')
            .style('fill', '#1565c0')
            .attr('r', 3);

        this.focus.append('circle')
            .attr('class', 'y')
            .style('fill', '#82b1ff')
            .style('opacity', 0.25)
            .attr('r', 6);

        this.graph.append('rect')
            .attr('width', this.width)
            .attr('height', this.height)
            .attr('class', 'overlay')
            .on('mouseover', () => {
                this.focus.style('display', null);
                self.tooltip.transition()
                    .duration(250)
                    .style('opacity', 0.9);
            })
            .on('mouseout', () => {
                this.focus.style('display', 'none');
                this.tooltip.transition()
                    .duration(500)
                    .style('opacity', 0);
            })
            .on('mousemove', function () {
                const x0 = self.x.invert(d3.mouse(this)[0]);
                const i = d3.bisector((data: any) => data.timestamp).left(self.data, x0, 1, self.data.length - 1);
                const d0 = self.data[i - 1];
                const d1 = self.data[i];

                const d = x0.getTime() - new Date(d0.timestamp).getTime() > new Date(d1.timestamp).getTime() - x0.getTime() ? d1 : d0;

                self.dimensions = self.container.node().getBoundingClientRect();

                self.focus.selectAll('circle.y')
                    .attr('transform', 'translate(' + self.x(d.timestamp) + ',' + self.y(d.agents) + ')');

                self.focus.select('.x')
                    .attr('transform', 'translate(' + self.x(d.timestamp) + ',' + self.y(d.agents) + ')')
                    .attr('y2', self.height - self.y(d.agents));

                self.focus.select('.y')
                    .attr('transform', 'translate(' + self.width * -1 + ',' + self.y(d.agents) + ')')
                    .attr('x2', 2 * self.width);

                self.tooltip.html(d3.timeFormat(self.getTimeFormat(true))(new Date(d.timestamp)) + '<br/>'
                        + d.agents + ' Agent'
                    + (d.agents === 1 ? '' : 's'))
                    .style('left', self.dimensions.left + self.margin.left + d3.mouse(this)[0] + 15 + 'px')
                    .style('top', self.dimensions.top + window.pageYOffset + d3.mouse(this)[1] - 25 + 'px');
            });
    }
}
