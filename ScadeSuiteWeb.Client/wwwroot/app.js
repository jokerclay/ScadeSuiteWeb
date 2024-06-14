// Blazor Just talks to JavaScript, so we can use the window object to access the X6 library.
(function (global) {
    global.initializeGraph = function () {
        // console.log('app.js is running');
        const container = document.getElementById('container');
        // console.log(container);
        if (container) {
            const { Graph, Shape } = global.X6;
            const { Selection } = global.X6PluginSelection;
            const { Scroller  } = global.X6PluginScroller;
            const { Snapline  } = global.X6PluginSnapline;

            const graph = new Graph({
                container: container,
                width: 1500,
                height:800,
                background: {color: '#F2F7FA'},
                mousewheel: {
                    enabled: true,
                    zoomAtMousePosition: true,
                    modifiers: 'ctrl',
                    minScale: 0.5,
                    maxScale: 3,
                },
                connecting: {
                    router: 'orth',
                    connector: {
                        name: 'rounded',
                        args: {
                            radius: 8,
                        },
                    },
                    anchor: 'center',
                    connectionPoint: 'anchor',
                    allowBlank: false,
                    snap: {
                        radius: 20,
                    },
                    createEdge() {
                        return new Shape.Edge({
                            attrs: {
                                line: {
                                    stroke: '#A2B1C3',
                                    strokeWidth: 2,
                                    targetMarker: {
                                        name: 'block',
                                        width: 12,
                                        height: 8,
                                    },
                                },
                            },
                            zIndex: 0,
                        })
                    },
                    validateConnection({sourceCell, targetCell, sourceMagnet, targetMagnet,})
                    {

                        const SourcePortId = sourceMagnet.getAttribute('port')
                        const TargetPortId = targetMagnet .getAttribute('port')
                        let SourceNodeType = getPortIdFirstPart(SourcePortId)
                        let TargetNodeType = getPortIdFirstPart(TargetPortId)

                        // 同一种节点不能相连
                        if (SourceNodeType === TargetNodeType) return false;

                        // output 节点不能连接到 函数 节点
                        if (SourceNodeType  === "Out" && TargetNodeType.startsWith("Func")  ) return false;

                        // output 节点不能连接到  输入 节点
                        if (SourceNodeType  === "Out" && TargetNodeType.startsWith("In")  ) return false;

                        // Func 节点不能连接到  输入 节点
                        if (SourceNodeType  === "Func" && TargetNodeType.startsWith("In")  ) return false;

                        // 输入只能有一个连接

                        // 不能重复连线
                        const edges = this.getEdges()
                        if (edges.find((edge) => edge.getTargetPortId() === TargetPortId)) {
                            return false
                        }

                        // 不能连接自身
                        return sourceCell !== targetCell;
                    }
                },

                grid: {
                    visible: true,
                    type: 'doubleMesh',
                    args: [
                        {
                            color: '#eee', // 主网格线颜色
                            thickness: 1, // 主网格线宽度
                        },
                        {
                            color: '#ddd', // 次网格线颜色
                            thickness: 1, // 次网格线宽度
                            factor: 4, // 主次网格线间隔
                        },
                    ],
                },

                highlighting: {
                    // 连接桩可以被连接时在连接桩外围围渲染一个包围框
                    magnetAvailable: {
                        name: 'stroke',
                        args: {
                            attrs: {
                                fill: '#fff',
                                stroke: '#A4DEB1',
                                strokeWidth: 4,
                            },
                        },
                    },
                    // 连接桩吸附连线时在连接桩外围围渲染一个包围框
                    magnetAdsorbed: {
                        name: 'stroke',
                        args: {
                            attrs: {
                                fill: '#fff',
                                stroke: '#31d0c6',
                                strokeWidth: 4,
                            },
                        },
                    },
                },
            })

            let selection = new Selection({enabled: true, multiple: true, rubberband: true, movable: true, showNodeSelectionBox: true,})
            let scroller = new Scroller({enabled: true,})
            let snapline = new Snapline({enabled: true, sharp: true,})
            
            Graph.registerNode(
                'custom-node-width-port',
                {
                    inherit: 'rect',
                    width: 100,
                    height: 40,
                    attrs: {
                        body: {
                            stroke: '#8f8f8f',
                            strokeWidth: 1,
                            fill: '#fff',
                            rx: 6,
                            ry: 6,
                        },
                    },
                    ports: {
                        groups: {
                            top: {
                                position: 'top',
                                attrs: {
                                    circle: {
                                        magnet: true,
                                        stroke: '#8f8f8f',
                                        r: 5,
                                    },
                                },
                            },
                            left: {
                                position: 'left',
                                attrs: {
                                    circle: {
                                        magnet: true,
                                        stroke: '#8f8f8f',
                                        r: 5,
                                    },
                                },
                            },
                            right: {
                                position: 'right',
                                attrs: {
                                    circle: {
                                        magnet: true,
                                        stroke: '#8f8f8f',
                                        r: 5,
                                    },
                                },
                            },
                            bottom: {
                                position: 'bottom',
                                attrs: {
                                    circle: {
                                        magnet: true,
                                        stroke: '#8f8f8f',
                                        r: 5,
                                    },
                                },
                            },
                        },
                    },
                },
                true,
            )

            function getPortIdFirstPart(str) {
                // Split the string by underscores
                const parts = str.split('_');
                // Return the first part
                return parts[0];
            }

            const input1 = graph.addNode({
                shape: 'custom-node-width-port',
                x: 140,
                y: 160,
                label: 'input1',
                ports: {
                    items: [
                        {
                            id: 'In_Port_1',
                            group: 'right',
                        },
                    ],
                },
            })





            const input2 = graph.addNode({
                shape: 'custom-node-width-port',
                x: 140,
                y: 240,
                label: 'input2',
                ports: {
                    items: [
                        {
                            id: 'In_Port_1',
                            group: 'right',
                        },
                    ],
                },
            })


            const output = graph.addNode({
                shape: 'custom-node-width-port',
                x: 560,
                y: 200,
                label: 'Output1',
                ports: {
                    items: [
                        {
                            id: 'Out_Port_1',
                            group: 'left',
                        },
                    ],
                },
            })

            const addFunc = graph.addNode({
                shape: 'custom-node-width-port',
                width: 150,
                height: 80,
                x: 330,
                y: 200,
                label: 'addFunc',
                ports: {
                    items: [
                        {
                            id: 'Func_Add_Port_1',
                            group: 'left',
                            attrs: {
                                text: {
                                    id: 'Func_Add_Port_1_Text',
                                    text: '1',
                                },
                            },
                        },
                        {
                            id: 'Func_Add_Port_2',
                            group: 'left',
                            attrs: {
                                text: {
                                    id: 'Func_Add_Port_2_Text',
                                    text: '2',
                                },
                            },
                        },
                        {
                            id: 'Func_Add_Port_3',
                            group: 'right',
                            attrs: {
                                text: {
                                    id: 'Func_Add_Port_3_Text',
                                    text: '3',
                                },
                            },
                        },
                    ],
                },
            })


// zoom
            graph.bindKey(['ctrl+1', 'meta+1'], () => {
                const zoom = graph.zoom()
                if (zoom < 1.5) {
                    graph.zoom(0.1)
                }
            })
            graph.bindKey(['ctrl+2', 'meta+2'], () => {
                const zoom = graph.zoom()
                if (zoom > 0.5) {
                    graph.zoom(-0.1)
                }
            })

            var selected_node = null;
            selection.on('node:selected', ({ node }) => {
                console.log('selected node:', node)
                console.log('node ports ==> :', node.port.ports.length)
                selected_node = node;
            })

            var vs =[];
            graph.on('view:mounted', ({ view }) => {
                vs.push(view)
            })

// console.log(graph)

//delete
            document.onkeydown = function (e) {
                if(e.key ==="Backspace") {
                    const cells = graph.getSelectedCells()
                    console.log(cells)
                    if (cells.length) {
                        graph.removeCells(cells)
                    }
                }
            };

            graph.on('edge:click', ({ e, x, y, edge, view }) => {
                console.log('e', e, )
                console.log('x',  x, )
                console.log('y',  y, )
                console.log('edge',  edge)
                console.log('view',  view)
            })

            graph.use(selection)
            graph.use(scroller)
            graph.use(snapline)
        } else {
            console.error('Container element not found');
        }
    };
    

})(window)


