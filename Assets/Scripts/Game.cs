using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

static class Game
{
    public static StreamingLog log = new StreamingLog();
    public static Graph graph = new Graph();

    static Game()
    {
        log.Publish("graph-events", graph.events);
    }

}
