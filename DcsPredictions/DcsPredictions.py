import numpy as np
import matplotlib.pyplot as plt
from sklearn.svm import SVC

# Assume data is x, z, side
data = np.array([
    [4500, -1512, 0],
    [5234, -2523, 0],
    [1242, -3112, 0],
    [1000, -20, 0],
    [2351, 200, 1],
    [2341, -123, 1],
    [5234, 20, 1],
    [5235, 20, 1],
    [2341, -2342, 0],
    [4000, 400, 1]
])

nContourPoints = 20
margin = 5000

X = data[:, :2]
y = data[:, 2]

clf = SVC(kernel='rbf', C=2)
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
Z = clf.decision_function(xy).reshape(XX.shape)

contour = ax.contour(XX, YY, Z, colors='k', levels=[0], alpha=0.5, linestyles=['-'])

# Get the contour points that pyplot created
contour_points = np.concatenate([
    path.vertices
    for collection in contour.collections
    for path in collection.get_paths()
])

for point in contour_points:
    plt.scatter(point[0], point[1], c='r', s=10)

plt.show()

if __name__ == '__main__':
    pass