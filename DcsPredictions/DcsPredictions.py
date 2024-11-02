import sys
import datetime
import json
import numpy as np
import matplotlib.pyplot as plt
from sklearn.neighbors import KNeighborsClassifier
# from sklearn.svm import SVC


if __name__ == '__main__':
    if len(sys.argv) != 2:
        print("Arguments missing")
        sys.exit(1)

    dt = datetime.datetime.now().strftime("%Y%m%d_%H%M%S")
    with open(f"./TestOutput/pythonOut{dt}.txt", "w") as file:
        file.write(sys.argv[1])

    data = np.array(json.loads(sys.argv[1]))

    nContourPoints = 300
    margin = 500000

    X = data[:, :2]
    y = data[:, 2]

    # clf = SVC(kernel='rbf', C=12)
    clf = KNeighborsClassifier(n_neighbors=1)
    clf.fit(X, y)

    plt.scatter(X[:, 0], X[:, 1], c=y, s=30, cmap=plt.cm.Paired)

    ax = plt.gca()

    xlim = ax.get_xlim()
    ylim = ax.get_ylim()

    ax.set_xlim([xlim[0] - margin, xlim[1] + margin])
    ax.set_ylim([ylim[0] - margin, ylim[1] + margin])

    xlim = ax.get_xlim()
    ylim = ax.get_ylim()

    # this code creats a grid of points that coveres the whole graph
    xx = np.linspace(xlim[0], xlim[1], nContourPoints)
    yy = np.linspace(ylim[0], ylim[1], nContourPoints)
    YY, XX = np.meshgrid(yy, xx)
    xy = np.vstack([XX.ravel(), YY.ravel()]).T

    # Do a prediction of each of the points
    Z = clf.predict(xy).reshape(XX.shape)
    # Z = clf.decision_function(xy).reshape(XX.shape)

    contour = ax.contour(XX, YY, Z, colors='k', levels=[(Z.min() + Z.max()) / 2], alpha=0.5, linestyles=['-'])

    # Get the contour points that pyplot created
    contour_points = np.concatenate([
        path.vertices
        for collection in contour.collections
        for path in collection.get_paths()
    ])

    for point in contour_points:
        plt.scatter(point[0], point[1], c='r', s=10)

    plt.savefig(f"./TestOutput/plot_{dt}.png")

    json.dump({"Points": contour_points.tolist()}, sys.stdout)
    

